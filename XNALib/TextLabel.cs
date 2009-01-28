using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace XNALib
{
    public class TextLabel : DrawableGameComponent
    {
        Rectangle textBox;
        SpriteFont font;
        String text;
        String parsedText;
        String typedText;
        double typedTextLength;
        int delayInMilliseconds;
        bool isDoneDrawing;
        SpriteBatch spriteBatch;
        Texture2D debugColor; 
        public TextLabel(Game game) : base(game)
        {
            textBox = new Rectangle(10, 10, 200, 40);
        }
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);  
            debugColor = Game.Content.Load<Texture2D>("solidred");
            font = Game.Content.Load<SpriteFont>("Fonts/GameFont");
            //text = "This is a properly implemented word wrapped sentence in XNA using SpriteFont.MeasureString. A B C D E F G H I J K L M N O P Q R S T U V W X Y Z a b c d e f g h i j k l m n o p q r s t u v w x y z";
            text = "Hello";
            
            parsedText = parseText(text);
            delayInMilliseconds = 50;
            isDoneDrawing = false;  

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (!isDoneDrawing)
            {
                if (delayInMilliseconds == 0)
                {
                    typedText = parsedText;
                    isDoneDrawing = true;
                }
                else if (typedTextLength < parsedText.Length)
                {
                    typedTextLength = typedTextLength + gameTime.ElapsedGameTime.TotalMilliseconds / delayInMilliseconds;

                    if (typedTextLength >= parsedText.Length)
                    {
                        typedTextLength = parsedText.Length;
                        isDoneDrawing = true;
                    }

                    typedText = parsedText.Substring(0, (int)typedTextLength);
                }
            }  

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState);
            spriteBatch.Draw(debugColor, textBox, Color.White);  
            spriteBatch.DrawString(font, typedText, new Vector2(textBox.X, textBox.Y), Color.White);
            spriteBatch.End();  

            base.Draw(gameTime);
        }

        private String parseText(String text)
        {
            String line = String.Empty;
            String returnString = String.Empty;
            String[] wordArray = text.Split(' ');

            foreach (String word in wordArray)
            {
                if (font.MeasureString(line + word).Length() > textBox.Width)
                {
                    returnString = returnString + line + '\n';
                    line = String.Empty;
                }

                line = line + word + ' ';
            }

            return returnString + line;
        }  
    }
}
