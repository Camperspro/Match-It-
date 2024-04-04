using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace MatchIt
{
    public class GameState
    {
        private int table = 1;
        private int pairs = 0;
        private int hearts = 4;
        string theme = "none";
        private string state = "NG"; //NG - New Game | GS - Game Start | GO - Game Over | NT - Next Table
        public GameState() 
        { 
        }

        public GameState(string _theme)
        { this.theme = _theme; }
        
        public int getPairs() { return pairs; }
        public int getTable() { return table; }
        public string getState() { return state; }
        public int getHearts() {  return hearts; }
        public string getTheme() { return theme; }

        public void setPairs(int p) { pairs = p; }
        public void setTable(int p) {  table = p; }
        public void setState(string s) { state = s; }
        public void setHearts(int p) {  hearts = p; }
        public void setTheme(string theme) { this.theme = theme; }

        public void reset()
        {
            pairs = 0;
            table = 1;
            hearts = 4;
            state = "NG";
        }

    }
}
