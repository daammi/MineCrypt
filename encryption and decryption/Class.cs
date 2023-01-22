using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetroFramework.Forms;
using MySql.Data.MySqlClient;
using System.Data;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace encryption_and_decryption
{
    /// <summary>
    /// 랜덤 패스워드 생성 클래스
    /// </summary>
    public class Get_key
    {
        // 랜덤 패스워드 생성 매서드
        public static string Key(int Length)
        {
            const string chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

            Random rand = new Random();
            StringBuilder rand_key = new StringBuilder();

            for (int i = 0; i < Length; i++)
            {
                //StringBuilder.Append : 현재 StringBuilder의 끝에 정보를 추가
                //rand.Next(max value) : max value까지의 임의의 정수를 반환
                rand_key.Append(chars[rand.Next(chars.Length)]);
            }
            //ToString 매서드 : 현재 객채를 나타내는 문자열을 반환
            return rand_key.ToString();
        }
    }

    /// <summary>
    /// DES 암호화 알고리즘 클래스
    /// </summary>
    public class DES
    {
        //암복호화시 필요한 변수
        public readonly byte[] salt = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        public const int iterations = 1042;

        /// <summary>
        /// 암호화 매서드
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="outputFile"></param>
        public void Encrypt(string inputFile, string outputFile)
        {
            try
            {
                //키 설정
                string KEY = "kit2022!";
                //유니코드인코딩 객체 생성
                UnicodeEncoding UE = new UnicodeEncoding();
                //키를 바이트로 변환
                byte[] key = UE.GetBytes(KEY);

                string cryptFile = outputFile;

                //***초안에선 StreamReader를 사용***
                //FileStream 클래스 객체 FS 생성 -> 원본 파일을 암호화하여 값을 저장할 파일
                //FileMode.Create
                FileStream FS = new FileStream(cryptFile, FileMode.Create);

                //RijndaelManaged 객체 RM 생성
                RijndaelManaged RM = new RijndaelManaged();

                //CryptoStream 객체 CS 생성
                //CryptoStreamMode.Write
                CryptoStream CS = new CryptoStream(FS, RM.CreateEncryptor(key, key), CryptoStreamMode.Write);

                //FileStream 클래스 객체 FS_in생성 -> 원본 파일
                // FileMode.Open
                FileStream FS_in = new FileStream(inputFile, FileMode.Open);

                //CS를 통해 FS에 암호화된 값을 바이트로 저장
                int data;
                while ((data = FS_in.ReadByte()) != -1)
                    CS.WriteByte((byte)data);

                FS_in.Close();
                CS.Close();
                FS.Close();

                //inputFile 삭제
                File.Delete(inputFile);
            }
            catch (Exception e)
            {
                MessageBox.Show("Error: {0}", e.Message);
            }
        }
        /// <summary>
        /// 복호화 매서드
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="outputFile"></param>
        public void Decrypt(string inputFile, string outputFile)
        {
            {
                try
                {
                    //키 설정
                    string KEY = "kit2022!";
                    //유니코드인코딩 객체 생성
                    UnicodeEncoding UE = new UnicodeEncoding();
                    //키를 바이트로 변환
                    byte[] key = UE.GetBytes(KEY);

                    //***초안에선 StreamReader를 사용***
                    //FileStream 클래스 객체 FS 생성 -> 암호화된 파일
                    //FileMode.Open
                    FileStream FS = new FileStream(inputFile, FileMode.Open);

                    //RijndaelManaged 객체 RM 생성
                    RijndaelManaged RM = new RijndaelManaged();

                    //CryptoStream 객체 CS 생성
                    //CryptoStreamMode.Read
                    CryptoStream CS = new CryptoStream(FS, RM.CreateDecryptor(key, key), CryptoStreamMode.Read);

                    //FileStream 클래스 객체 FS_out생성 -> 복호화된 내용이 저장될 파일
                    // FileMode.Create
                    FileStream FS_out = new FileStream(outputFile, FileMode.Create);

                    //CS를 통해 FS_out에 복호화된 값을 바이트로 저장
                    int data;
                    while ((data = CS.ReadByte()) != -1)
                        FS_out.WriteByte((byte)data);
 
                    FS_out.Close();
                    CS.Close();
                    FS.Close();

                    // inputFile 삭제
                    File.Delete(inputFile);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Error: {0}", e.Message);
                }

            }
        }
    }
}

