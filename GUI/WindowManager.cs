﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GUI
{
    /// <summary>
    /// Deals with displaying and interaction of windows - all GUI action happens here.
    /// </summary>
    public class WindowManager
    {
        List<Window> Windows;
        public Window MovingWindow;
        public float MoveX;
        public float MoveY;
        public int MouseX;
        public int MouseY;
        public Vector2 Screen;
        public Interfaces.ITextInput FocusedText;
        public ToolTip ToolTip;
        public float MouseStillSeconds;
        public IAction MouseGrab;
        public Control LastClickedControl;
        public string NotificationText;
        public float NotificationTimeout;
        float NotificationTime = 100;
        public WindowManager()
        {
            this.Windows = new List<Window>();
          //  this.ToolTip = new ToolTip(); fuck tooltips!
        }
        /// <summary>
        /// Updates timers, events etc
        /// </summary>
        /// <param name="seconds">Seconds since last update. Might not be accurate.</param>
        public void Update(float seconds)
        {
            if (this.MovingWindow != null)
            {

                this.MovingWindow.X = (int)MathHelper.Clamp(this.MouseX - this.MoveX, 0, Screen.X - this.MovingWindow.Width);
                this.MovingWindow.Y = (int)MathHelper.Clamp(this.MouseY - this.MoveY, 0, Screen.Y - this.MovingWindow.Height);

            }
            this.NotificationTimeout = Math.Max(0, this.NotificationTimeout - seconds);
        }
        public void Notify(string Text, bool Override = false)
        {
            if (Override || this.NotificationTimeout == 0)
            {
                this.NotificationText = Text;
                this.NotificationTimeout = NotificationTime;

            }
        }
        public void Render(GraphicsDevice device)
        {
            foreach (Window Window in this.Windows)
            {
                Window.Render(device, 0, 0);
            }
            if (this.ToolTip != null)
            {
                if (MouseStillSeconds >= 70f)
                {
                    if (this.ToolTip.Visible == false)
                    {
                        this.ToolTip.X = this.MouseX;
                        this.ToolTip.Y = this.MouseY;
                        this.ToolTip.Visible = true;
                    }
                    this.ToolTip.Render(device);
                }
                else
                {
                    this.ToolTip.Visible = false;
                }
            }
            if (this.MouseGrab != null)
            {
                this.MouseGrab.Render(device, MouseX - 16, MouseY - 16);

            }
            if (this.NotificationText != null && this.NotificationTimeout > 0)
            {
                int CX = (int)((float)device.Viewport.Width / 2f);
                int CY = (int)((float)device.Viewport.Height / 2f);
                float scale = NotificationTime / 2.0f;
                float A = Math.Min(1.0f, this.NotificationTimeout / this.NotificationTime);
                Color c = new Color(1.0f, 1.0f, 0.9f, A);
                GUIDraw.RenderBigText(device, CX, CY, this.NotificationText, c, true);
            }
        }
        public void Add(Window Window)
        {
            Window.WM = (WindowManager)this;
            Window.WM.GetType();
            this.Windows.Add(Window);
        }
        public bool HandleMouse(MouseState Mouse)
        {
            Window Window = GetWindow(Mouse.X, Mouse.Y);
            if (Window == null)
            {
                if (this.MovingWindow != null)
                    return true;
                if (this.MouseGrab != null)
                    return true;
                return false;
            }
            Window.MouseMove(MouseX - Window.X, MouseY - Window.Y);
            bool MouseIsDown = Mouse.LeftButton == ButtonState.Pressed;
            if (MouseIsDown)
            {
                if (Volatile.KeyMask[64000] == false) //the mouse was up last time
                {
                    Window.MouseDown(MouseX - Window.X, MouseY - Window.Y);
                }
                Volatile.KeyMask[64000] = true;

            }
            else
            {
                if (Volatile.KeyMask[64000] == true) //the mouse was down last time
                {
                    Window.MouseUp(MouseX - Window.X, MouseY - Window.Y);
                }
                Volatile.KeyMask[64000] = false;

            }


            return true;
        }
        public void Click(Window Window, float X, float Y)
        {

            // this.Windows[idxc] = tmp;
            Window.Click(X - Window.X, Y - Window.Y);
            MoveX = X - Window.X;
            MoveY = Y - Window.Y;

        }
        public void Top(Window Window)
        {
            Window tmp;
            int idxc = this.Windows.LastIndexOf(Window);
            tmp = this.Windows[idxc];
            for (int i = idxc; i < this.Windows.Count - 1; i++)
            {
                this.Windows[i] = this.Windows[i + 1];
            }
            this.Windows[this.Windows.Count - 1] = Window;
        }
        /*
        public void MouseMove(float X, float Y)
        {
            Window Window = this.GetWindow(X, Y);
            if (Window == null)
            {
                this.ToolTip = null;
                return;
            }
            Window.MouseMove(X - Window.X, Y - Window.Y);
        }
        //*/
        public Window GetWindow(float X, float Y)
        {
            Window wnd = null;

            for (int i = this.Windows.Count - 1; i >= 0; i--)
            {
                if (this.Windows[i].CheckCollision(X, Y))
                    return this.Windows[i];
            }

            return wnd;
        }
    }

    public static class WindowSettings
    {
        public static Color WindowColour = new Color(102, 26, 0);

    }

}
