using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MatchIt
{
    public class Card
    {
        int value = 0;
        bool isFlipped = false;
        BitmapImage image = null;

        public Card()
        {
            value = 0;
            image = null;
        }
        public Card(int value)
        {
            this.value = value;
        }

        public Card(BitmapImage image)
        {
            this.image = image;
        }

        public void setValue(int nValue) { this.value = nValue; }
        public void setImage(BitmapImage image) { this.image = image; }
        public void setFlipped(bool flip) { this.isFlipped = flip; }
        public int getValue() { return value; }
        public BitmapImage getImage() { return image; }
        public bool getFlipped() { return isFlipped; }

    }
}
