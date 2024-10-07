using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MarkSheet
{
    public partial class Form1 : Form
    {
        public SQLiteConnection myConnection;
        private object database;


        /*private object dataGridView1;*/
        public Form1()
        {
            InitializeComponent();
        }

        public void connect()
        {
            myConnection = new SQLiteConnection("Data Source = C:\\Users\\Anas Shaker\\Desktop\\New folder (2)\\database.db");

            myConnection.Open();

        }

        private void button4_Click(object sender, EventArgs e)
        {
            connect();
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            comboBox1.ResetText();
            lGradeTextBox.Clear();
        }


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            connect();
            SQLiteCommand command = new SQLiteCommand();
            command.CommandText = "INSERT INTO markSheet (termInformation, courseName, courseCode, ECTS, lGrade, nGrade) VALUES (@terminformation, @courseName, @coursecode, @ects, @lgrade, @ngrade)";
            command.Connection = myConnection;
            command.Parameters.AddWithValue("@courseName", textBox1.Text);
            command.Parameters.AddWithValue("@coursecode", textBox2.Text);
            command.Parameters.AddWithValue("@ects", textBox3.Text);
            command.Parameters.AddWithValue("@ngrade", textBox4.Text);
            command.Parameters.AddWithValue("@lgrade", lGradeTextBox.Text);
            command.Parameters.AddWithValue("@terminformation", comboBox1.Text);
            //command.Parameters.AddWithValue("", comboBox1.Text);
            command.ExecuteNonQuery();
            button5_Click(sender, e);
            myConnection.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            connect();
            SQLiteCommand command = new SQLiteCommand();
            command.CommandText = "SELECT termInformation, courseName, courseCode, ECTS, lGrade, nGrade from markSheet";
            command.Connection = myConnection;
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            dataGridView1.DataSource = dt;
            myConnection.Close();
        }

        /*GPA-CGA*/
        private void button7_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
            {
                GPACal();
            }
            else if (radioButton2.Checked == true)
            {
                MessageBox.Show("Your CGPA is: " + CGPACal());
            }
            else
            {
                MessageBox.Show("Seriously??\nChoose a term to calculate GPA in it!");
            }
            button5_Click(sender, e);
            myConnection.Close();
        }
        public string lGPACalculater(int gpa)
        {
            if (gpa >= 90 && gpa <= 100)
            {
                return "AA";
            }
            else if (gpa >= 85 && gpa <= 89.99)
            {
                return "BA";
            }
            else if (gpa >= 80 && gpa <= 84.99)
            {
                return "BB";
            }
            else if (gpa >= 75 && gpa <= 79.99)
            {
                return "CB";
            }
            else if (gpa >= 70 && gpa <= 74.99)
            {
                return "CC";
            }
            else if (gpa >= 60 && gpa >= 69.99)
            {
                return "DC";
            }
            else if (gpa >= 50 && gpa >= 59.99)
            {
                return "DD";
            }
            else if (gpa >= 0 && gpa >= 49.99)
            {
                return "FF";
            }
            return "GF";
        }

        public void GPACal()
        {
            connect();
            SQLiteCommand command = (SQLiteCommand)myConnection.CreateCommand();
            command.CommandText = "SELECT nGrade FROM markSheet WHERE TermInformation = '" + comboBox1.Text + "'";
            SQLiteDataReader sqReader = command.ExecuteReader();

            string gpa;
            int sum = 0, count = 0;
            double avg;

            if (comboBox1.Text != "")
            {
                // Always call Read before accessing data.
                while (sqReader.Read())
                {
                    gpa = (string)sqReader.GetValue(0); //string to get it is a value from the DataBase
                    sum += Convert.ToInt32(gpa);
                    count++;
                }

                if (count > 0)
                {
                    avg = sum / count;
                    MessageBox.Show("GPA in Term " + comboBox1.Text + " is:\n" + avg.ToString(), "Aha, now you started to understand :)");
                }
                else
                {
                    MessageBox.Show("Sorry there are no grades in this term yet\nChoose another term pls", "Bruh!!");
                }
            }
            else
            {
                MessageBox.Show("Pleasae Select a term to calculate GPA in it.\nOr who know what I can do, i am smart program @@", "STOP searching for Errors!!");
            }

            sqReader.Close(); // always call Close when done reading.
            myConnection.Close(); // Close the connection when done with it.
        }

        public string CGPACal()
        {
            connect();

            double Grades;
            int ECTS;

            SQLiteCommand command = new SQLiteCommand("SELECT ECTS, nGrade from Marksheet order by termInformation", myConnection);


            SQLiteDataReader current = command.ExecuteReader();
            int numOFRows = 0;
            while (current.Read())
            {
                numOFRows++;
            }
            current.Close();


            SQLiteDataReader reader = command.ExecuteReader();

            double[] Marks = new double[numOFRows];
            double[] credithours = new double[numOFRows];
            double[] TGPA = new double[numOFRows];

            int i = 0;
            while (reader.Read())
            {

                Grades = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("nGrade")));
                Marks[i] = Grades;
                ECTS = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("ECTS")));
                credithours[i] = ECTS;
                i++;
            }

            for (int j = 0; j < numOFRows; j++)
            {
                if (Marks[j] >= 90 && Marks[j] <= 100)
                {
                    TGPA[j] = 4.00 * credithours[j];
                }
                else if (Marks[j] >= 85 && Marks[j] <= 89.99)
                {
                    TGPA[j] = 3.50 * credithours[j];
                }
                else if (Marks[j] >= 80 && Marks[j] <= 84.99)
                {
                    TGPA[j] = 3.00 * credithours[j];
                }
                else if (Marks[j] >= 75 && Marks[j] <= 79.99)
                {
                    TGPA[j] = 2.50 * credithours[j];
                }
                else if (Marks[j] >= 70 && Marks[j] <= 74.99)
                {
                    TGPA[j] = 2.00 * credithours[j];
                }
                else if (Marks[j] >= 60 && Marks[j] <= 69.99)
                {
                    TGPA[j] = 1.50 * credithours[j];
                }
                else if (Marks[j] >= 50 && Marks[j] <= 59.99)
                {
                    TGPA[j] = 1.00 * credithours[j];
                }
                else if (Marks[j] > 0.00 && Marks[j] <= 49.99)
                {
                    TGPA[j] = 0.00 * credithours[j];
                }
                else
                {

                }
            }

            double GPA = 0;
            double TC = 0;
            double ClassGpa = 0;

            for (int x = 0; x < numOFRows; x++)
            {
                GPA = GPA + TGPA[x];
                TC = TC + credithours[x];
                ClassGpa = GPA / TC;
            }

            return ClassGpa.ToString("N2");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            connect();
            string courseName = textBox1.Text;
            SQLiteCommand sqlCmd = new SQLiteCommand();
            sqlCmd.CommandText = "Delete from marksheet where coursename='" + courseName + "'";
            sqlCmd.Connection = myConnection;

            sqlCmd.ExecuteNonQuery();
            button5_Click(sender, e);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            connect();
            SQLiteCommand command = new SQLiteCommand();
            command.CommandText = "SELECT * FROM marksheet WHERE termInformation = '" + comboBox1.Text + "'";
            command.Connection = myConnection;
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            dataGridView1.DataSource = dt;
            myConnection.Close();

        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            connect();
            SQLiteCommand command = new SQLiteCommand();
            command.CommandText = "SELECT * FROM marksheet WHERE CourseName = '" + textBox1.Text + "'";
            command.Connection = myConnection;
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            dataGridView1.DataSource = dt;
            myConnection.Close();

        }

     
    }
}
