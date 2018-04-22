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
     
        bool connected;
        GameClient gclient;
        // int numberOfPlayers;
        //private Image bgrImage;
        List<int[]> players = new List<int[]>();
        string dbname = "C:/Users/polina/Downloads/Quadrario/Quadrario/playerdb.db3";

        public GameWindow()
        {
            InitializeComponent();
            connected = false;
        }
        private void GameWindow_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.W || e.KeyChar == (char)119 
                || e.KeyChar == (char)Keys.A || e.KeyChar == (char)97
                || e.KeyChar == (char)Keys.S || e.KeyChar == (char)115
                || e.KeyChar == (char)Keys.D || e.KeyChar == (char)100)
            {
                gclient.RequestReply_Move(e.KeyChar.ToString());
                players.Clear();
                SQLiteConnection cnnect = new SQLiteConnection("Data Source=" + dbname + ";Version=3;");
                cnnect.Open();
                SQLiteCommand command = new SQLiteCommand();
                command.Connection = cnnect;
                command.CommandText = @"SELECT * FROM [players]";

                try
                {
                    SQLiteDataReader r = command.ExecuteReader();
                    string line = String.Empty;
                    while (r.Read())
                    {
                        int[] playerInfo = new int[4];
                        playerInfo[0] = Convert.ToInt32(r["id"]);
                        playerInfo[1] = Convert.ToInt32(r["xcoordinate"]);
                        playerInfo[2] = Convert.ToInt32(r["ycoordinate"]);
                        playerInfo[3] = Convert.ToInt32(r["size"]);
                        players.Add(playerInfo);  
                    }
                    r.Close();
                }
                catch (SQLiteException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                cnnect.Close();
                RefreshGame();
                DrawPlayers();
            }


        }
        protected void RefreshGame()
        {
            System.Drawing.Graphics g;
            g = this.CreateGraphics();
            using (SolidBrush sBrush = new SolidBrush(Color.White))
            {
                g.FillRectangle(sBrush, 10, 50, this.Size.Width, this.Size.Height);

            }
            g.Dispose();
            /*
            ShapeContainer canvas = new ShapeContainer();
            RectangleShape bgr = new RectangleShape();
            canvas.Parent = this;
            bgr.Parent = canvas;
            bgr.Size = new System.Drawing.Size(660, 500);
            bgr.Location = new System.Drawing.Point(10,50);
            bgr.BackColor = Color.White;
            bgr.BackStyle = BackStyle.Opaque;
            bgr.BorderColor = Color.White;
            */
        }
        protected void DrawPlayers() {
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
        }
        /*protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            /*using (Pen selPen = new Pen(Color.White))
            {
                g.DrawRectangle(selPen, 100, 200, 500, 500);
            }*/

            /*System.Drawing.SolidBrush myBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);
            System.Drawing.Graphics formGraphics;
            formGraphics = this.CreateGraphics();
            formGraphics.FillRectangle(myBrush, new Rectangle(10, 100, 660, 450));
            myBrush.Dispose();
            formGraphics.Dispose();

            Color randomColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
            int[] playerInfo = new int[4];
            for (int i = 0; i < 4; i++) {
                playerInfo[0] = rnd.Next(100);
                playerInfo[1] = rnd.Next(10, 670);
                playerInfo[2] = rnd.Next(100, 550);
                playerInfo[3] = rnd.Next(50, 200);
                players.Add(playerInfo);
                using (Pen selPen = new Pen(randomColor))
                {
                    g.DrawRectangle(selPen, playerInfo[1], playerInfo[2], playerInfo[3], playerInfo[3]);
                }
            }

        }*/
       
        
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
                    connected = true;
                    if (NotifLabel.CanFocus)
                    {
                        NotifLabel.Focus();
                    }

                    SQLiteConnection cnnect = new SQLiteConnection("Data Source=" + dbname + ";Version=3;");
                    cnnect.Open();
                    SQLiteCommand command = new SQLiteCommand();
                    command.Connection = cnnect;
                    command.CommandText = @"INSERT INTO [players]([id], [xcoordinate], [ycoordinate], [size]) "
                                + "VALUES (" + userId + ", 335, 300, 40);";
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();

                    command.CommandText = @"SELECT * FROM [players]";
                    
                    Color randomColor;
                    System.Drawing.Graphics g;
                    g = this.CreateGraphics();
                    try
                    {
                        SQLiteDataReader r = command.ExecuteReader();
                        string line = String.Empty;
                        while (r.Read())
                        {
                            int[] playerInfo = new int[4];
                            randomColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                            playerInfo[0] = Convert.ToInt32(r["id"]);
                            playerInfo[1] = Convert.ToInt32(r["xcoordinate"]);
                            playerInfo[2] = Convert.ToInt32(r["ycoordinate"]);
                            playerInfo[3] = Convert.ToInt32(r["size"]);
                            players.Add(playerInfo);
                        }
                        r.Close();
                    }
                    catch (SQLiteException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    cnnect.Close();
                    DrawPlayers();
                    gclient = new GameClient(IPTextbox.Text){ Id = userId, IP = IPTextbox.Text };

                    //while (true)
                    //{
                    //    Thread.Sleep(10);
                    //}
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