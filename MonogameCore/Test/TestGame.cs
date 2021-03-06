﻿using System;
using System.Collections.Generic;
using Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Threading.Tasks;

namespace MonogameCore.Test
{
    public class TestGame : GameState
    {
        public TestGame() : base() { }

        public override void Load(SpriteBatch batch)
        {
            lineRenderer.Add(new Line(new Vector2(0), new Vector2(16, 16), Color.Red));
            lineRenderer.Add(new Line(new Vector2(0), new Vector2(0, 16)));
            lineRenderer.Add(new Line(new Vector2(16), new Vector2(16, 0)));
            lineRenderer.Add(new Line(new Vector2(16), new Vector2(0, 16)));
            //UI
            SpriteFont font = AssetManager.GetResource<SpriteFont>("mainFont");
            Text text = new Text(this, "Position: ", new Vector2(0f, 0f), new Vector2(16f, 1f), font);
            text.colour = new Color(0, 255, 0);
            text.tag = "positionText";
            Button button = new Button(this, "Menu!", "block", () => GameStateManager.RequestChange("menu", CHANGETYPE.LOAD),
                font, new Vector2(14, 0), new Vector2(2, 1));
            button.SetupColours(Color.Gray, Color.White, Color.DarkGray, Color.Red);
            //Objects
            GameObject stone0 = new GameObject("stone", this, 2, true);
            stone0.Pos = new Vector2(0, 8);
            stone0.Size = new Vector2(8, 1);
            stone0.AddComponent(new CRender("block"));
            stone0.AddComponent(new CAABB());
            GameObject stone1 = new GameObject("stone", this, 2, true);
            stone1.Pos = new Vector2(9, 7);
            stone1.Size = new Vector2(2, 2);
            stone1.AddComponent(new CTileableSprite("tiletest", 2, 2));
            stone1.AddComponent(new CAABB());
            GameObject stone2 = new GameObject("stone", this, 2, true);
            stone2.Pos = new Vector2(12, 5);
            stone2.Size = new Vector2(3, 0.2f);
            stone2.AddComponent(new CRender("block"));
            stone2.AddComponent(new CAABB());
            GameObject stone3 = new GameObject("stone", this, 2, true);
            stone3.Pos = new Vector2(8, 3);
            stone3.Size = new Vector2(3, 0.2f);
            stone3.AddComponent(new CRender("block"));
            stone3.AddComponent(new CAABB());
            GameObject killer = new GameObject("killer", this, 2);
            killer.AddComponent(new CRender("suprise"));
            killer.AddComponent(new CAABB());
            killer.Pos = new Vector2(3, 5);
            killer.Size = new Vector2(1, 1);
            (killer.Renderer as CRender).colour = Color.Red;
            GameObject player = new GameObject("player", this, 1);
            player.AddComponent(new CRender("dude"));
            player.AddComponent(new CPlayerMovement(3.0f));
            player.AddComponent(new CAABB());
            player.AddComponent(new CShoot());
            player.AddComponent(new CHealthBar(5, player));
            player.Pos = new Vector2(1, 1);
            player.Size = new Vector2(0.5f, 1.0f);
            GameObject anim = new GameObject("rotationtest", this, 5);

            for(int i = 0; i < 100; i++)
            {
                GameObject s = new GameObject("stone", this, 2, true);
                s.Pos = new Vector2(12+(float)MathH.random.NextDouble()*3, (float)MathH.random.NextDouble() * 3);
                s.Size = new Vector2(1, 1);
                s.AddComponent(new CRender("block"));
                s.AddComponent(new CAABB());
            }

            CAnimatedSprite animatie = new CAnimatedSprite();
            animatie.AddAnimation("letters", "animLetters");
            animatie.AddAnimation("nummers", "animNumbers");
            animatie.PlayAnimation("letters", 4);
            anim.AddComponent(animatie);
            anim.Pos = new Vector2(5, 0);
            anim.Size = new Vector2(1, 1);
            GameObject floor = new GameObject("stone", this, 2, true);
            floor.Pos = new Vector2(-100, 8);
            floor.Size = new Vector2(100, 1);
            floor.AddComponent(new CRender("block"));
            floor.AddComponent(new CAABB());
            AudioManager.PlayTrack("music");
            AudioManager.LoopTrack(true);
            AudioManager.SetTrackVolume(1f);
            AudioManager.SetEffectVolume(0.3f);
            AudioManager.SetMasterVolume(0f);
            Debug.FullDebugMode();
        }
        
        public override void Unload() { }

        public override void Update(float time)
        {
            Camera.SetCameraTopLeft(new Vector2(0, 0));
            Text text = ui.FindWithTag("positionText") as Text;
            GameObject player = objects.FindWithTag("player");

            text.text[0] = ("Position: " + MathH.Float(player.Pos.X, 2) + " , " + MathH.Float(player.Pos.Y, 2));

            if (Input.GetKey(PressAction.PRESSED, Keys.P))
            {
                if (Debug.Mode == DEBUGMODE.PROFILING)
                    Debug.FullDebugMode();
                else Debug.ProfilingMode();
            }
            if (Input.GetKey(PressAction.DOWN, Keys.O)) Debug.showAtlas = true;
            else Debug.showAtlas = false;
            if (Input.GetKey(PressAction.PRESSED, Keys.Y))
            {
                GameObject go = objects.FindWithTag("rotationtest");
                (go.Renderer as Renderer).Rotate(10f);
            }
            if (Input.GetKey(PressAction.PRESSED, Keys.Q))
            {
                Task.Factory.StartNew(test);
                Task.Factory.StartNew(test);
            }
            if (Input.GetMouseButton(PressAction.PRESSED, MouseButton.LEFT))
                AudioManager.PlayEffect("bleep");
            base.Update(time);
        }
        
        private void test()
        {
            for (int i = 0; i < 100; i++)
            {
                GameObject a = new GameObject(this, 0);
                a.Pos = new Vector2((float)MathH.random.NextDouble() * 16, (float)MathH.random.NextDouble() * 16);
                a.Size = new Vector2(0.5f);
                a.AddComponent(new CRender("block"));
                a.AddComponent(new CAABB());
            }
        }

        public override void Draw(float time, SpriteBatch batch, GraphicsDevice device)
        {
            base.Draw(time, batch, device);
        }
    }
}