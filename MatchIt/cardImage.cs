using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MatchIt
{
    class cardImage : ObserverObject
    {
        private BitmapImage cImage = new BitmapImage();

        public BitmapImage Cimage
        {
            get { return cImage; }
            set
            { 
                cImage = value;
                OnPropertyChanged("Cimage");
            }
        }
    }
}
