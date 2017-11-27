﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
//Simpele GameStateManager, spreekt voorzich.
namespace Core
{
    public abstract class GameState
    {
        public GameObjectManager objects;
        public UIObjectManager ui;
        internal Collision collision;
        internal LayeredRenderer renderer;
        internal bool loaded = false;

        public GameState()
        {
            objects = new GameObjectManager();
            ui = new UIObjectManager();
            collision = new Collision();
            renderer = new LayeredRenderer();
        }
        
        public abstract void Load(SpriteBatch batch);
        public abstract void Unload();
        public virtual void Update(float time)
        {
            Timers.Update(time);
            objects.Update(time);
            collision.Check();
            ui.Update();
        }
        public virtual void Draw(float time, SpriteBatch batch, GraphicsDevice device)
        {
            device.Clear(Color.Black);
            batch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Camera.TranslationMatrix);
            renderer.Render();
            batch.End();
            batch.Begin();
            ui.Draw(batch);
            batch.End();
        }
        public bool Loaded { get { return loaded; } }
    }
    
    public enum CHANGETYPE
    {
        LOAD,
        SWITCH
    }

    public class GameStateManager
    {
        private Dictionary<string, GameState> states;
        private GameState currentstate;
        private static GameStateManager instance;
        private SpriteBatch batch;

        internal GameStateManager(SpriteBatch batch)
        {
            if (instance != null) return;
            states = new Dictionary<string, GameState>();
            currentstate = null;
            this.batch = batch;
            instance = this;
        }

        private void SetState(string name)
        {
            if (states.ContainsKey(name))
                currentstate = states[name];
        }

        public static void RequestChange(string state, CHANGETYPE type)
        {
            if (type == CHANGETYPE.LOAD && instance.currentstate != null)
            {
                instance.currentstate.Unload();
                instance.currentstate.ui.Clear();
                instance.currentstate.objects.Clear();
                instance.currentstate.collision.Clear();
                instance.currentstate.loaded = false;
                Time.timeScale = 1.0f;
            }
            instance.SetState(state);
            instance.currentstate.loaded = true;
            if (type == CHANGETYPE.LOAD) instance.currentstate.Load(instance.batch);
        }

        internal void Update(float time)
        {
            Input.Update();
            if (currentstate == null) return;
            currentstate.Update(time);
        }

        internal void Draw(float time, SpriteBatch batch, GraphicsDevice device)
        {
            if (currentstate == null) return;
            currentstate.Draw(time, batch, device);
        }

        public void AddState(string name, GameState state)
        {
            if (state == null) return;
            if (states.ContainsKey(name)) return;
            states.Add(name, state);
        }

        public void RemoveState(string name)
        {
            if (states.ContainsKey(name))
                states.Remove(name);
        }

        public void SetStartingState(string name)
        {
            if (currentstate != null) return;
            SetState(name);
            currentstate.Load(batch);
            currentstate.loaded = true;
        }

        public static GameState CurrentState { get { return instance.currentstate; } }
    }
}