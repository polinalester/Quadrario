using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;
using System.Data.Common;
using System.Data.SQLite;
using System.Threading;


namespace GameClient
{
    public partial class GameWindow : Form
    {
        private Random rnd = new Random();
        GameClient gclient;
        public List<int[]> players = new List<int[]>();
        string dbname = "C:/Users/polina/Downloads/Quadrario/Quadrario/playerdb.db3";

        public GameWindow()
        {
            InitializeComponent();
        }
        private void GameWindow_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.W || e.KeyChar == (char)119 
                || e.KeyChar == (char)Keys.A || e.KeyChar == (char)97
                || e.KeyChar == (char)Keys.S || e.KeyChar == (char)115
                || e.KeyChar == (char)Keys.D || e.KeyChar == (char)100)
            {
                gclient.RequestReply_Move(e.KeyChar.ToString());
                RefreshGame();
                DrawPlayers();
            }

            if (e.KeyChar == (char)32)
            {
                gclient.RequestReply_Intersect();
                RefreshGame();
                DrawPlayers();
            }
        }
        public void RefreshGame()
        {
            System.Drawing.Graphics g;
            g = this.CreateGraphics();
            using (SolidBrush sBrush = new SolidBrush(Color.White))
            {
                g.FillRectangle(sBrush, 10, 50, this.Size.Width, this.Size.Height);

            }
            g.Dispose();
        }
        public void DrawPlayers() {
            Color randomColor;
            System.Drawing.Graphics g;
            g = this.CreateGraphics();
            for (int i = 0; i < players.Count; i++)
            {
                randomColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                using (SolidBrush sBrush = new SolidBrush(randomColor))
                {
                    g.FillRectangle(sBrush, players[i][1], players[i][2], players[i][3], players[i][3]);
                }
                Font drawFont = new Font("Arial", 8);
                using (SolidBrush drawBrush = new SolidBrush(Color.Black))
                {
                    StringFormat drawFormat = new StringFormat();
                    g.DrawString(players[i][0].ToString(), 
                        drawFont, drawBrush, 
                        players[i][1], 
                        players[i][2], 
                        drawFormat);
                }
                drawFont.Dispose();
            }
            g.Dispose();
            NotifLabel.Text = players.Count.ToString();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            var userId = 0;
            TextBox IDtextbox = (TextBox)IDTextbox;
            if (!Int32.TryParse(IDtextbox.Text, out userId))
            {
                NotifLabel.Text = "Please insert valid ID.";
                IDTextbox.Clear();
            }
            else
            {
                Regex ipParser = new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b");
                TextBox IPtextbox = (TextBox)IPTextbox;
                MatchCollection result = ipParser.Matches(IPtextbox.Text);
                if (!(result.Count == 1))
                {
                    IPTextbox.Clear();
                    NotifLabel.Text = "Please insert valid IP.";
                    IPTextbox.Clear();

                }
                else {
                    var match = result[0];
                    NotifLabel.Text = "Connecting from user " + userId + " to " + match.Value;
                    if (NotifLabel.CanFocus)
                    {
                        NotifLabel.Focus();
                    }
                    //TODO: LoginService
                    SQLiteConnection cnnect = new SQLiteConnection("Data Source=" + dbname + ";Version=3;");
                    cnnect.Open();
                    SQLiteCommand command = new SQLiteCommand();
                    command.Connection = cnnect;
                    command.CommandText = @"INSERT INTO [players]([id], [xcoordinate], [ycoordinate], [size]) "
                                + "VALUES (" + userId + ", 335, 300, 40);";
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                    cnnect.Close();
                    gclient = new GameClient(IPTextbox.Text, this){ Id = userId, IP = IPTextbox.Text };
                }
            }
        }

        private void GameWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            if(gclient != null)
                gclient.Dispose();
        }

        private void GameWindow_Load(object sender, EventArgs e)
        {

        }
    }
}