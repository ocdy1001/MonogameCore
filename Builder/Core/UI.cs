﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
//Buttons, text, etc. Simpel en spreekt voor zich.
namespace Core
{
    public static class UI
    {
        public static void TextInCenter(String s, Vector2 offset, Vector2 size, SpriteBatch batch, SpriteFont font, Color col)
        {
            batch.DrawString(font, s, (size / 2f) + offset, col, 0, font.MeasureString(s) / 2f, 1f, SpriteEffects.None, 1f);
        }
    }

    public abstract class UIElement : _tagged
    {
        protected Vector2 position;
        private Vector2 size, sizemul;
        internal bool dirtysize = true;
        public Color colour;
        public bool active;
        public GameObject GO;
        protected Vector2 offset = Vector2.Zero;
        private GameState context;

        public UIElement(GameState context, Vector2 position, Vector2 size)
        {
            this.position = position;
            this.size = size;
            colour = Color.White;
            context.ui.Add(this);
            this.context = context;
            active = true;
        }

        public UIElement(Vector2 position, Vector2 size, Color colour)
        {
            this.position = position;
            this.size = size;
            this.colour = colour;
            active = true;
        }

        public virtual void Update()
        {
            if (!active)
                return;
            if (GO != null)
                position = GO.Pos;
        }
        
        public void AddGameObject(GameObject GO, Vector2 offset)
        {
            this.GO = GO;
            this.offset = offset;
        }

        protected Vector2 DrawSize(Vector2 physicalsize)
        {
            if (dirtysize)
            {
                sizemul = size * Grid.ScaleSprite(physicalsize);
                dirtysize = false;
            }
            return sizemul;
        }

        public Vector2 Size
        {
            get { return size; }
            set { size = value; dirtysize = true; }
        }

        public Vector2 Pos
        {
            get { return position; }
            set { position = value; }
        }

        public virtual void Draw(SpriteBatch batch)
        {
            if (!active)
                return;
        }

        public void Destroy()
        {
            context.ui.Remove(this);
        }
    }

    public class UITextureElement : UIElement
    {
        protected Texture2D texture;

        public UITextureElement(GameState context, string texture, Vector2 position, Vector2 size)
            : base(context, position, size)
        {
            this.texture = AssetManager.GetResource<Texture2D>(texture);
        }

        public override void Draw(SpriteBatch batch)
        {
            base.Draw(batch);
            Vector2 drawsize = DrawSize(new Vector2(texture.Width, texture.Height));
            batch.Draw(texture, Grid.ToScreenSpace(position + offset), null, 
                colour, 0.0f, Vector2.Zero, drawsize, 
                SpriteEffects.None, 0.0f);
        }
    }

    public class Text : UIElement
    {
        public List<string> text = new List<string>();
        protected SpriteFont font;
        AABB bounds;

        public Text(GameState context, string text, Vector2 position, Vector2 size, SpriteFont font) 
            : base(context, position, size)
        {
            this.text.Add(text);
            this.font = font;
        }

        public Text(GameState context, List<string> text, Vector2 position, Vector2 size, SpriteFont font)
            : base(context, position, size)
        {
            this.text = text;
            this.font = font;
            bounds = new AABB(position.X, position.Y, size.X, size.Y);
        }

        public override void Draw(SpriteBatch batch)
        {
            base.Draw(batch);
            UI.TextInCenter(text[0], Grid.ToScreenSpace(position + offset), Grid.ToScreenSpace(Size), batch, font, colour);

        }

        public bool Hover { get { return bounds.Inside(Input.GetMousePosition()); } }
        public bool Clicked { get { return Hover && Input.GetMouseButton(PressAction.PRESSED, MouseButton.LEFT); } }
    }

    public class MultipleLinesText : UIElement
    {
        public List<string> text;
        protected SpriteFont font;
        protected AABB bounds;
        protected Vector2 size;
        public int selected = -1;

        public MultipleLinesText(GameState context, List<string> text, Vector2 position, Vector2 size, SpriteFont font) 
            : base(context, position, size)
        {
            this.text = text;
            this.font = font;
            //this.size = size;
            //temporary size until we have scalable letters
            string longest = "";
            foreach(string s in text)
            {
                if (font.MeasureString(s).X > font.MeasureString(longest).X)
                    longest = s;
            }
            this.size = Grid.ToGridSpace(new Vector2(font.MeasureString(longest).X, text.Count * font.MeasureString("This is just a random string, for testing purposes").Y));

            bounds = new AABB(position.X, position.Y, this.size.X, this.size.Y);
        }

        public override void Draw(SpriteBatch batch)
        {
            base.Draw(batch);
            if (!active)
                return;
            for (int i = 0; i < text.Count; i++)
                batch.DrawString(font, text[i], Grid.ToScreenSpace(position) + new Vector2((Grid.ToScreenSpace(size).X - font.MeasureString(text[i]).X)/2, i * font.MeasureString(text[i]).Y), colour);

            //batch.DrawString(text[i], Grid.ToScreenSpace(position) + new Vector2(0, i * font.MeasureString(text[i]).Y), Grid.ToScreenSpace(new Vector2(Size.X, Size.Y / text.Count)), font, colour);

        }

        //returns the line that the mouse is over/that is clicked, or -1
        public int Hover
        {
            get
            {
                if (!active || !bounds.Inside(Input.GetMousePosition()))
                    return -1;
                return (int)((Input.GetMousePosition().Y - position.Y) /(size.Y/text.Count));
            }
        }

        public int Clicked
        {
            get
            {
                if (!active || !bounds.Inside(Input.GetMousePosition()) || !Input.GetMouseButton(PressAction.PRESSED, MouseButton.LEFT))
                    return -1;
                return (int)((Input.GetMousePosition().Y - position.Y) / (Grid.ToScreenSpace(size).Y/text.Count));
            }
        }

        public Vector2 Pos { get { return position; } set { position = value; bounds = new AABB(position.X, position.Y, this.size.X, this.size.Y); } }
        new public Vector2 Size { get { return size; } }
    }

    public class SliderBar : UITextureElement
    {
        private SliderButton button;

        public SliderBar(GameState context, string barTexture, string buttonTexture, int buttonSize, Vector2 position, Vector2 size, string axis): base(context, barTexture, position, size)
        {
            if (axis == "x")
                button = new SliderButton(context, buttonTexture, position - new Vector2(0, (buttonSize - size.Y)) /2, new Vector2(buttonSize), axis, position.X, size.X);
            else
                button = new SliderButton(context, buttonTexture, position - new Vector2((buttonSize - size.X), 0) /2, new Vector2(buttonSize), axis, position.Y, size.Y);
        }

        public float Value
        {
            get { return button.Value; }
            set { button.Value = (float)MathH.Clamp(value, 0f, 1f); }
        }
    }

    internal class SliderButton : UITextureElement
    {
        protected string axis;
        protected float start, limit;
        protected Vector2 grabPoint, mousePos, size;
        protected AABB bounds;
        protected bool grabbed, staticGrabbed;

        internal SliderButton(GameState context, string texture, Vector2 position, Vector2 size, string axis, float start, float limit): base(context, texture, position, size)
        {
            this.axis = axis;
            this.limit = limit;
            this.start = start;
            this.size = size;
            bounds = new AABB(position.X, position.Y, size.X, size.Y);
        }

        public override void Update()
        {
            mousePos = Input.GetMousePosition();
            bounds = new AABB(position.X, position.Y, size.X, size.Y);
            if (Input.GetMouseButton(PressAction.PRESSED, MouseButton.LEFT))
            {
                if (bounds.Inside(mousePos))
                {
                    if (!staticGrabbed)
                    {
                        staticGrabbed = true;
                        grabbed = true;
                        grabPoint = mousePos - position;
                    }
                }
            }
            else if (Input.GetMouseButton(PressAction.RELEASED, MouseButton.LEFT))
            {
                grabbed = staticGrabbed = false;
                grabPoint = Vector2.Zero;
            }

            if (grabbed)
            {
                if(axis == "x")
                    position.X = Math.Min(Math.Max(start - size.X / 2, mousePos.X - grabPoint.X), start + limit - size.X / 2);
                else if (axis == "y")
                    position.Y = Math.Min(Math.Max(start - size.Y / 2, mousePos.Y - grabPoint.Y), start + limit - size.Y / 2);
            }
        }

        //returns a value between 0-1
        public float Value
        {
            get
            {
                if (axis == "x")
                    return (position.X + (size.Y / 2) - start) / limit;
                else
                    return (position.Y + (size.Y / 2) - start) / limit;
            }
            set
            {
                if (axis == "x")
                    position.X = start - (size.Y / 2) + (limit * value);
                else
                    position.Y = start - (size.Y / 2) + (limit * value);
            }
        }

    }

    public class Button : UITextureElement
    {
        public Color baseColour, highlightColour, downColour, textColour;
        protected string text;
        protected AABB bounds;
        protected Action pressAction;
        protected SpriteFont font;

        public Button(GameState context, string text, string texture, Action a, SpriteFont font, Vector2 position, Vector2 size) 
            : base(context, texture, position, size)
        {
            colour = Color.White;
            highlightColour = Color.White;
            downColour = Color.White;
            textColour = Color.Black;
            baseColour = Color.White;
            this.text = text;
            this.texture = AssetManager.GetResource<Texture2D>(texture);
            bounds = new AABB(position.X, position.Y, size.X, size.Y);
            pressAction = a;
            this.font = font;
        }

        public void SetupColours(Color baseColour, Color highlightColour, Color downColour, Color textColour)
        {
            this.baseColour = baseColour;
            this.highlightColour = highlightColour;
            this.downColour = downColour;
            this.textColour = textColour;
            colour = baseColour;
        }

        public override void Update()
        {
            if (!active)
                return;
            Vector2 mpos = Input.GetMousePosition();
            bool down = Input.GetMouseButton(PressAction.DOWN, MouseButton.LEFT);
            bool pressed = Input.GetMouseButton(PressAction.RELEASED, MouseButton.LEFT);
            bool hover = bounds.Inside(mpos);
            if (hover && pressed && pressAction != null) pressAction();
            if (hover && down) colour = downColour;
            else if (hover) colour = highlightColour;
            else colour = baseColour;
        }

        public override void Draw(SpriteBatch batch)
        {
            base.Draw(batch);
            UI.TextInCenter(text, Grid.ToScreenSpace(position), Grid.ToScreenSpace(Size), batch, font, textColour);
        }
    }
}