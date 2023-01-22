using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using MetroFramework.Forms;
using MySql.Data.MySqlClient;

namespace encryption_and_decryption
{
    public partial class Form1 : MetroForm
    {
        string key = string.Empty;
        //GUID 객체 생성
        string guid = Guid.NewGuid().ToString();

        //MariaDB target 서버;데이터베이스;UID;PWD
        string target = "Server=203.234.232.189;Database=Get_key;Uid=root;Pwd=kim5213;";

        //FolderBrowserDialog 전역변수 생성
        FolderBrowserDialog dialog = new FolderBrowserDialog();

        public Form1()
        {
            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //폼 로드시에 키 생성
            key = Get_key.Key(32);

            string guidPath = @"D:\guid.txt";

            FileInfo fileInfo = new FileInfo(guidPath);
            if (!fileInfo.Exists)
            {
                // Text 파일 생성 및 text 를 입력 합니다.
                File.WriteAllText(guidPath, guid, Encoding.Default);
            }
            else
            {
                guid = File.ReadAllText(guidPath);
            }
        }

        //프로그램 종료 매서드
        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnPage1Next_Click(object sender, EventArgs e)
        {
            panel1.Visible = true;
        }

        private void btnPage2Next_Click(object sender, EventArgs e)
        {
            panel2.Visible = true;
        }

        private void btnPage2Prev_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
        }

        private void btnPage2Run_Click(object sender, EventArgs e)
        {

                //박스에 값이 없을때 실행X
                if (checkedListBox1.SelectedItem != null)
                {
                    if (checkedListBox1.CheckedItems.Count != 0)
                    {
                        if (MessageBox.Show("정말 암호화하시겠습니까?", "암호화 확인", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            run(key, true);
                            MessageBox.Show("암호화 완료");
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("선택된 파일이 없습니다.");
                    }

                }
                else
                {
                    MessageBox.Show("리스트에 파일이 존재하지않습니다.");
                }
 
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            //체크박스1의 value가 true일때 checkall 매서드 호출
            if (checkBox1.Checked == true)
                checkall1();
            else
                uncheckall1();
        }

        private void btnPage2Folder_Click(object sender, EventArgs e)
        {
            //선택한 폴더를 체크파일리스트에 띄움
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                //path를 셀렉된 위치로 지정
                string path = dialog.SelectedPath;

                //checkedListBox1 Reset
                this.checkedListBox1.Refresh();

                //DirectoryInfo 객체 생성
                DirectoryInfo di = new DirectoryInfo(path);

                if (di.Exists == true)
                {
                    //DT 객체 생성
                    DataTable dt = new DataTable();
                    //객체 dt에 컬럼 추가
                    dt.Columns.Add("FileName", typeof(string));
                    dt.Columns.Add("FullName", typeof(string));

                    //DR 객체 생성 
                    DataRow ds;

                    foreach (var file in di.GetFiles())
                    {
                        string path_file = path + @"\" + file.Name;

                        if (Path.GetExtension(path_file) != ".locked")
                        {
                            //foreach문을 이용하여 각 ds에 파일네임과 파일풀네임 입력
                            ds = dt.NewRow();
                            ds["FileName"] = file.Name.ToString();
                            ds["FullName"] = file.FullName;
                            //dt에 ds행추가
                            dt.Rows.Add(ds);
                        }
                    }

                    //체크리스트박스의 데이터소스를 dt로 지정
                    checkedListBox1.DataSource = dt;
                    checkedListBox1.DisplayMember = "FileName";
                    checkedListBox1.ValueMember = "FullName";

                }
                else
                {
                    MessageBox.Show("해당 폴더가 존재하지 않습니다.");
                    return;
                }
                //폴더에 파일이 들어오면 true 반환
            }
        }

        private void btnPage3Prev_Click(object sender, EventArgs e)
        {
            panel2.Visible = false;
        }

        private void btnPage3Run_Click(object sender, EventArgs e)
        {
            string decKey = textBox1.Text;

            if (checkedListBox2.SelectedItem != null)
            {
                if (textBox1.Text == "")
                {
                    MessageBox.Show("키를 입력해주세요.");
                    return;
                }
                else if (textBox1.Text != key)
                {
                    MessageBox.Show("입력하신 키가 일치하지 않습니다.");
                    textBox1.Text = null;
                    return;
                }
                else
                {
                    if (checkedListBox2.CheckedItems.Count != 0)
                    {
                        if (MessageBox.Show("정말 복호화하시겠습니까?", "복호화 확인", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            run(decKey, false);
                            MessageBox.Show("복호화 완료");
                        }
                        else
                        {
                            return;
                        }
                    }
                    else {
                        MessageBox.Show("체크된 파일이 없습니다.");
                    }
                }
            }
            else
            {
                MessageBox.Show("리스트에 파일이 존재하지않습니다.");
            }

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            //체크박스2의 value가 true일때 checkall 매서드 호출
            if (checkBox2.Checked == true)
                checkall2();
            else
                uncheckall2();
        }

        private void btnPage3Folder_Click(object sender, EventArgs e)
        {
            //선택한 폴더를 체크파일리스트에 띄움
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                //path를 셀렉된 위치로 지정
                string path = dialog.SelectedPath;

                //checkedListBox1 Reset
                this.checkedListBox2.Refresh();

                //DirectoryInfo 객체 생성
                DirectoryInfo di = new DirectoryInfo(path);

                if (di.Exists == true)
                {
                    //DT 객체 생성
                    DataTable dt = new DataTable();
                    //객체 dt에 컬럼 추가
                    dt.Columns.Add("FileName", typeof(string));
                    dt.Columns.Add("FullName", typeof(string));

                    //DR 객체 생성 
                    DataRow ds;

                    foreach (var file in di.GetFiles())
                    {
                        string path_file = path + @"\" + file.Name;

                        if (Path.GetExtension(path_file) == ".locked")
                        {
                            //foreach문을 이용하여 각 ds에 파일네임과 파일풀네임 입력
                            ds = dt.NewRow();
                            ds["FileName"] = file.Name.ToString();
                            ds["FullName"] = file.FullName;
                            //dt에 ds행추가
                            dt.Rows.Add(ds);
                        }
                    }

                    //체크리스트박스의 데이터소스를 dt로 지정
                    checkedListBox2.DataSource = dt;
                    checkedListBox2.DisplayMember = "FileName";
                    checkedListBox2.ValueMember = "FullName";

                }
                else
                {
                    MessageBox.Show("해당 폴더가 존재하지 않습니다.");
                    return;
                }
            }
        }

        /// <summary>
        /// 암/복호화 실행 매서드
        /// </summary>
        /// <param name="key">키파일</param>
        /// <param name="type">암호화 선택 => true, 복호화 선택 => false </param>
        void run(string key, bool type)
        {
            //path를 다이로그에서 선택된 폴더로 설정
            string path = dialog.SelectedPath;
            //DirectoryInfo 객체 생성
            var dir = new DirectoryInfo(path);
            //AES 클래스 객체 생성
            var des = new DES();

            //selection2 카운트 세기
            int count=0;

            //List클래스 객체 생성
            List<string> selection = new List<string>();
            List<string> selection2 = new List<string>();

            //foreach문으로 selection에 순차적으로 file.Name 저장
            foreach (var file in dir.GetFiles())
            {
                string filePath = path + @"\" + file.Name;
                //foreach문으로 폴더내 파일이름 순차적으로 출력
                Console.WriteLine(file.Name);

                if (Path.GetExtension(filePath) == ".locked")
                {
                    selection2.Add(file.Name.ToString());
                    count++;
                }
                else
                {
                    selection.Add(file.Name.ToString());
                }
            }

            if (type)
            {
                using (MySqlConnection server = new MySqlConnection(target))
                {
                    server.Open();
                    //객체 생성및 MariaDB 서버로 키 전송
                    MySqlCommand msc = new MySqlCommand("insert into passwd values(now(),'" + guid + "','" + path + "','" + key + "')", server);
                    msc.ExecuteNonQuery();
                    server.Close();
                }
            }

            if (type)
            {
                    for (int i = 0; i < selection.Count; i++)
                    {
                        //GetitemChecked 메서드 => 체크가 되어있을때 true값 반환
                        if (checkedListBox1.GetItemChecked(i))
                        {
                            string input_file = path + @"\" + selection[i];
                            //원본 파일 이름에 .locked 추가하여 확장자 변경
                            string result_file = path + @"\" + selection[i] + ".locked";

                            //DES 암호화 매서드를 이용하여 파일내용 암호화
                            des.Encrypt(input_file, result_file);

                        }
                    }
               
            }
            else
            {
                    for (int i = 0; i < count; i++)
                    {
                        //GetitemChecked 메서드 => 체크가 되어있을때 true값 반환
                        if (checkedListBox2.GetItemChecked(i))
                        {
                            string input_file = path + @"\" + selection2[i];
                            //끝에서 7개만큼 지워서 원본 파일 확장자로 만듦
                            string result_file = path + @"\" + selection2[i].Remove(selection2[i].Length - 7);

                            //DES 복호화 매서드를 이용하여 파일내용 암호화
                            des.Decrypt(input_file, result_file);

                        }
                    }
            }
        }

        //체크박스 전체 선택/해제 매서드
        //각각 for문을 이용하여 item의 체크값을 전체 true OR false 입력
        public void checkall1()
        {
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                checkedListBox1.SetItemChecked(i, true);
            }
        }
        public void uncheckall1()
        {
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                checkedListBox1.SetItemChecked(i, false);
            }
        }
        public void checkall2()
        {
            for (int i = 0; i < checkedListBox2.Items.Count; i++)
            {
                checkedListBox2.SetItemChecked(i, true);
            }
        }
        public void uncheckall2()
        {
            for (int i = 0; i < checkedListBox2.Items.Count; i++)
            {
                checkedListBox2.SetItemChecked(i, false);
            }
        }

    }
}
