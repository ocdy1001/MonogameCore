﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Core
{
    public class UIObjectManager
    {
        private List<UIElement> elements;
        private TagEngine tags;

        internal UIObjectManager()
        {
            elements = new List<UIElement>();
            tags = new TagEngine();
        }

        internal void Update()
        {
            if(Grid.dirty > 0)
            {
                Grid.dirty--;
                for (int i = 0; i < elements.Count; i++)
                    elements[i].dirtysize = true;
            }
            for (int i = 0; i < elements.Count; i++)
                elements[i].Update();
        }

        internal void Draw(SpriteBatch batch)
        {
            for (int i = 0; i < elements.Count; i++)
                elements[i].Draw(batch);
        }

        internal void Add(UIElement e)
        {
            elements.Add(e);
        }

        internal void Remove(UIElement e)
        {
            elements.Remove(e);
        }

        internal void Clear()
        {
            elements.Clear();
        }

        public UIElement FindWithTag(string tag)
        {
            return tags.FindWithTag(tag, elements);
        }

        public UIElement[] FindAllWithTag(string tag)
        {
            return tags.FindAllWithTag(tag, elements);
        }

        public UIElement[] FindAllWithTags(string[] tags)
        {
            return this.tags.FindAllWithTags(tags, elements);
        }
    }
}