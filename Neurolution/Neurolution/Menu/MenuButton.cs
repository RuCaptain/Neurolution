using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Neurolution.Menu
{
    public class MenuButton : MenuItem
    {
        private readonly Texture2D _textureNormal;
        private readonly Texture2D _textureMouseOver;

        public MenuButton(Texture2D textureNormal, Texture2D textureOver) : base(textureNormal)
        {
            _textureNormal = textureNormal;
            _textureMouseOver = textureOver;
        }

        public void SetTexture(bool isOver)
        {
            Texture = isOver ? _textureMouseOver : _textureNormal;
        }

        public override event EventHandler Click;

        protected virtual void OnClick(EventArgs e)
        {
            var handler = Click;
            if (handler != null)
                handler(this, e);
        }

        public override void DoClick()
        {
            OnClick(new EventArgs());
        }

        public void SetPosition(Vector2 position)
        {
            Position = position;
        }


        public override string GetName()
        {
            return "menubutton";
        }

        public override void PerformAction(string action, List<object> parameters)
        {
            if(action == "mouseover")
                SetTexture((bool)parameters[0]);
        }

        public override Dictionary<string, object> GetCustomData()
        {
            return new Dictionary<string, object>();
        }
    }
}
