using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System;
using System.Text;
using UnityEngine.EventSystems;

namespace Exception//自作拡張?
{
    //以下音関連
    public static class AudioSystem //音を流すシステム
    {
        static AudioSource BGMAudio, SEAudio, VoiceAoudio;//BGM再生用　SE再生用 声再生用
        static GameObject BGM, SE, VOICE;
        public static void AudioSourceIN()
        {
            if (BGMAudio != null || SEAudio != null || VoiceAoudio != null) return;
            BGM = new GameObject("BGMAudio");
            SE = new GameObject("SEAudio");
            VOICE = new GameObject("VOICEAudio");

            BGM.AddComponent<AudioSource>();
            SE.AddComponent<AudioSource>();
            VOICE.AddComponent<AudioSource>();
            BGM.hideFlags = HideFlags.HideAndDontSave;//Hierarchyに表示しなくさせるが存在はしている
            SE.hideFlags = HideFlags.HideAndDontSave;//Hierarchyに表示しなくさせるが存在はしている
            VOICE.hideFlags = HideFlags.HideAndDontSave;//Hierarchyに表示しなくさせるが存在はしている

            BGMAudio = BGM.GetComponent<AudioSource>();
            SEAudio = SE.GetComponent<AudioSource>();
            VoiceAoudio = VOICE.GetComponent<AudioSource>();

            Option_Data OPdata = OptionDataSystem.Lord();//オプションデータ取得
            BGMAudio.volume = OPdata.BGMvolume;
            SEAudio.volume = OPdata.SEvolume;
            VoiceAoudio.volume = OPdata.Voicevolume;

            BGMAudio.playOnAwake = SEAudio.playOnAwake = VoiceAoudio.playOnAwake = false;
        }//再生機を作成

        public static void BGMVolumeIN(float volume) { BGMAudio.volume = volume; } //BGM音量変更

        public static void BGMPlaye(AudioClip BGM, bool Loop) { BGMAudio.clip = BGM; BGMAudio.loop = Loop; BGMAudio.Play(); }//BGM再生

        public static void BGMPause() { BGMAudio.Pause(); }//BGMを一時停止
        public static void BGMUnPause() { BGMAudio.UnPause(); }//BGMを一時停止終了

        public static void BGMStop() { BGMAudio.Stop(); }//BGM停止

        public static void SEVolumeIN(float volume) { SEAudio.volume = volume; }//SE音量変更

        public static void SEPlaye(AudioClip Se) { SEAudio.PlayOneShot(Se); }//SE再生

        public static void VoiceVolume(float volume) { VoiceAoudio.volume = volume; }//ボイスの音量変更

        public static void VoicePlaye(AudioClip Se) { VoiceAoudio.PlayOneShot(Se); }//ボイス再生
        public static void VoicePause() { VoiceAoudio.Pause(); }//ボイスを一時停止
        public static void VoiceUnPause() { VoiceAoudio.UnPause(); }//ボイスを一時停止終了

        public static void AllVolume(float BGMv, float SEv, float voicev) { BGMAudio.volume = BGMv; SEAudio.volume = SEv; VoiceAoudio.volume = voicev; }//BMG,SE,Voice音量同時変更
        public static void AudioSourceDes()
        { BGMAudio = null; SEAudio = null; VoiceAoudio = null; BGMAudio.playOnAwake = SEAudio.playOnAwake = VoiceAoudio.playOnAwake = true; }
    }
    //ここまで


    //以下通信関連
    public class Savar_Data//クライン受付サーバーのIPアドレスポート番号
    {
        public string IP = "";
        public int Port = 0;
    }

    public class SaverDataSystem//IPアドレスポート番号ロードセーブシステム
    {
        public static void Save(Savar_Data data)//IPアドレスセーブ
        {
            try
            {
                StreamWriter strw;
                string jos = JsonUtility.ToJson(data);
                strw = new StreamWriter(new FileStream(Application.dataPath + "/data/IP_Data.json", FileMode.Create));//ファイルがある場合上書き
                strw.Write(jos);
                strw.Flush();
                strw.Close();
            }
            catch
            {

                if (!Directory.Exists(Application.dataPath + "/data"))//フォルダーがある確認 ない場合フォルダー作成
                {
                    Debug.Log("保存先のフォルダーがないためフォルダーを作成します");
                    Directory.CreateDirectory(Application.dataPath + "/data");//フォルダー作成
                }
                Debug.Log("データを作成");
                StreamWriter strw;
                string jos = JsonUtility.ToJson(data);
                strw = new StreamWriter(new FileStream(Application.dataPath + "/data/IP_Data.json", FileMode.OpenOrCreate));
                strw.Write(jos);
                strw.Flush();
                strw.Close();
            }
        }

        public static Savar_Data Lord()//IPアドレスロード
        {
            try
            {
                StreamReader str;
                str = new StreamReader(new FileStream(Application.dataPath + "/data/IP_Data.json", FileMode.Open));
                string json = str.ReadToEnd();
                str.Close();
                return JsonUtility.FromJson<Savar_Data>(json);
            }
            catch
            {
                Savar_Data data = new Savar_Data();
                Save(data);

                StreamReader str;
                str = new StreamReader(new FileStream(Application.dataPath + "/data/IP_Data.json", FileMode.Open));
                string json = str.ReadToEnd();
                str.Close();
                return JsonUtility.FromJson<Savar_Data>(json);
            }
        }
    }

    public class Tuusin//自作通信システム
    {
        static Encoding ecn = Encoding.UTF8;//文字コード指定 
        static NetworkStream nst;//サーバーデータ
        static UdpClient souisinudp;//ゲームサーバールームデータ
        static UdpClient zyusinudp;//受信用

        public static void nstIN(NetworkStream ns)//サーバーデータを取得
        {
            nst = ns;
        }

        public static void tuusinKill()//通信エラーの場合サーバーデータを削除
        {
            nst = null;
            souisinudp = null;
        }

        public static void TCPKILL()
        {
            nst = null;
        }

        public static void UDPIN(UdpClient ud, UdpClient zudp)//UDPデータを代入
        {
            souisinudp = ud;
            zyusinudp = zudp;
        }

        public static void TCPsosin(string s)//データ送信処理
        {
            byte[] bun = ecn.GetBytes(s + '\n');//byteデータ作
            nst.Write(bun, 0, bun.Length);//送信
        }

        public static string TCPzyusi()//受信処理　タイムアウトあり
        {
            string s = "";
            nst.WriteTimeout = 1000;//受信タイムアウト設定　１秒
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    byte[] resByte = new byte[256];
                    int resSize = 0;//受信した文字数

                    do
                    {
                        resSize = nst.Read(resByte, 0, resByte.Length);//文字数記録
                        if (resSize == 0) break;

                        ms.Write(resByte, 0, resSize);//受信データ蓄積　文字　？　文字数
                    }
                    while (nst.DataAvailable || resByte[resSize - 1] != '\n');//読み取り可能データがあるか、データの最後が\nではない場合は受信を継続

                    string resMsg = ecn.GetString(ms.GetBuffer(), 0, (int)ms.Length);//受信データを文字列に変換
                    resMsg = resMsg.TrimEnd('\n');//文字最後の\nを消す
                    s = resMsg;
                }
            }
            catch
            {
                s = "11";
                Debug.Log("データなし");
            }
            return s;
        }

        public static string TCPzyusinTime_NO()//データ受信でタイムアウトなし版
        {
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] resByte = new byte[256];
                int resSize = 0;//受信した文字数

                do
                {
                    resSize = nst.Read(resByte, 0, resByte.Length);//文字数記録
                    if (resSize == 0) break;

                    ms.Write(resByte, 0, resSize);//受信データ蓄積　文字　？　文字数
                }
                while (nst.DataAvailable || resByte[resSize - 1] != '\n');//読み取り可能データがあるか、データの最後が\nではない場合は受信を継続

                string resMsg = ecn.GetString(ms.GetBuffer(), 0, (int)ms.Length);//受信データを文字列に変換
                resMsg = resMsg.TrimEnd('\n');//文字最後の\nを消す
                return resMsg;
            }
        }

        public static void UDPsousin(string s)//UDPでのデータ送信
        {
            byte[] bun = ecn.GetBytes(s);//送信データ作成
            souisinudp.Send(bun, bun.Length);//送信
        }

        public static string UDPzyusin()//UDPでの受信
        {
            IPEndPoint ep = null;
            byte[] zusin = zyusinudp.Receive(ref ep);
            return ecn.GetString(zusin);
        }
    }
    //ここまで

    public class ObjDataGet  //以下指定のタグのオブジェクトを全て取得させる処理
    {
        public static GameObject PlayerObjGet()//プレイヤーのタグが付いたオブジェクトを取得
        {
            GameObject RetumObj = null;
            foreach (GameObject obj in UnityEngine.Object.FindObjectsOfType(typeof(GameObject)))
            {
                if (obj.tag == "Player")
                {
                    RetumObj = obj;
                }
            }
            return RetumObj;
        }

        public static GameObject MainCameraObj()//メインカメラのタグが付いたオブジェクトを取得
        {
            GameObject RetumObj = null;
            foreach (GameObject obj in UnityEngine.Object.FindObjectsOfType(typeof(GameObject)))
            {
                if (obj.tag == "MainCamera")
                {
                    RetumObj = obj;
                }
            }
            return RetumObj;
        }

        public static List<GameObject> Tag_All_obj(string tag)//指定したタグの付いたオブジェクトリストを取得
        {
            List<GameObject> objdata = new List<GameObject>();
            List<GameObject> returnobjdata = new List<GameObject>();
            foreach (GameObject boj in UnityEngine.Object.FindObjectsOfType(typeof(GameObject)))
            {
                if (boj.tag == tag)
                {
                    objdata.Add(boj);
                }
            }
            for (int i = objdata.Count - 1; i > -1; i--)
            {
                returnobjdata.Add(objdata[i]);
            }
            return returnobjdata;
        }
    }
    //ここまで


    //以下操作キー操作コントローラーボタン設定関連
    public class Key_Data//初期操作キーデータ
    {
        //以下キー入力での操作
        public string Junp_Key = "Space";//ジャンプキー
        public string Hover_Key = "LeftShift";//ホバーキー
        public string Snaipakame_Key = "Mouse1";//スナイパー構え
        public string SnaipaShot_Key = "Mouse0";//スナイパー撃ち
        public string Attac_Key = "Mouse0";//攻撃キー 
        public string LookON_Key = "Tab";//ロックオンキー
        public string LookOFF_Key = "Q";//ロックオン解除キー
        public string Itemuse_Key = "E";//アイテム使用キー

        //以下コントローラーでの入力
        public string Junp_Con = "Joystick1Button0";//ジャンプボタン
        public string Hover_Con = "Joystick1Button8";//ホバーボタン
        public string Snaipakame_Con = "Joystick1Button4";//スナイパー構え
        public string SnaipaShot_Con = "Joystick1Button5";//スナイパー撃ち
        public string Attack_Con = "Joystick1Button1";//攻撃ボタン
        public string LookON_Con = "Joystick1Button9";//ロックオンボタン
        public string LookOFF_Con = "Joystick1Button3";//ロックオン解除ボタン
        public string Itemuse_Con = "Joystick1Button2";//アイテム使用ボタン
    }
    public class KeyDataSystem//操作キーデータロードセーブ
    {
        public static void Save(Key_Data data)//オプションセーブ
        {
            try
            {
                StreamWriter strw;
                string jos = JsonUtility.ToJson(data);
                strw = new StreamWriter(new FileStream(Application.dataPath + "/data/KeyData.json", FileMode.Create));//ファイルがある場合上書き
                strw.Write(jos);
                strw.Flush();
                strw.Close();
            }
            catch
            {

                if (!Directory.Exists(Application.dataPath + "/data"))//フォルダーがある確認 ない場合フォルダー作成
                {
                    Debug.Log("保存先のフォルダーがないためフォルダーを作成します");
                    Directory.CreateDirectory(Application.dataPath + "/data");//フォルダー作成
                }
                Debug.Log("データを作成");
                StreamWriter strw;
                string jos = JsonUtility.ToJson(data);
                strw = new StreamWriter(new FileStream(Application.dataPath + "/data/KeyData.json", FileMode.OpenOrCreate));
                strw.Write(jos);
                strw.Flush();
                strw.Close();
            }
        }

        public static Key_Data Lord()//オプションロード
        {
            try
            {
                StreamReader str;
                str = new StreamReader(new FileStream(Application.dataPath + "/data/KeyData.json", FileMode.Open));
                string json = str.ReadToEnd();
                str.Close();
                return JsonUtility.FromJson<Key_Data>(json);
            }
            catch
            {
                Key_Data data = new Key_Data();
                Save(data);

                StreamReader str;
                str = new StreamReader(new FileStream(Application.dataPath + "/data/KeyData.json", FileMode.Open));
                string json = str.ReadToEnd();
                str.Close();
                return JsonUtility.FromJson<Key_Data>(json);
            }
        }
    }

    public class MoveKey//移動のキーを返す
    {
        static string Hol = "Horizontal", Ver = "Vertical";
        static string MousHol = "Mouse X", MousVer = "Mouse Y";
        
        public static bool Horizontal_Right(){ bool b = false; if (Input.GetAxisRaw(Hol) > 0) b = true;  return b; }
        public static bool Horizonta_Left(){ bool b = false; if (Input.GetAxisRaw(Hol) < 0) b = true;  return b; }
        public static bool Vertical_Up(){ bool b = false; if (Input.GetAxisRaw(Ver) > 0) b = true;  return b; }
        public static bool Vertical_Down(){ bool b = false; if (Input.GetAxisRaw(Ver) < 0) b = true;  return b; }
        public static float Horizontal(){return Input.GetAxis(Hol);}
        public static float Vertical() {return Input.GetAxis(Ver);}

        public static bool Mous_Right() { bool b = false; if (Input.GetAxisRaw(MousHol) > 0) b = true; return b; }
        public static bool Mous_Left() { bool b = false; if (Input.GetAxisRaw(MousHol) < 0) b = true; return b; }
        public static bool Mous_Up() { bool b = false; if (Input.GetAxisRaw(MousVer) > 0) b = true; return b; }
        public static bool Mous_Down() { bool b = false; if (Input.GetAxisRaw(MousVer) < 0) b = true; return b; }
        public static float MousHorizontal() { return Input.GetAxis(MousHol); }
        public static float MousVertical() { return Input.GetAxis(MousVer); }
    }

    public class Input_Conversion//入力キー名をboolに変換
    {
        public static KeyCode Key_Conversion(string s)
        {
            KeyCode key = new KeyCode();
            switch (s)
            {
                case "Backspace":
                    key = KeyCode.Backspace;
                    break;
                case "Delete":
                    key = KeyCode.Delete;
                    break;
                case "Tab":
                    key = KeyCode.Tab;
                    break;
                case "Clear":
                    key = KeyCode.Clear;
                    break;
                case "Return":
                    key = KeyCode.Return;
                    break;
                case "Pause":
                    key = KeyCode.Pause;
                    break;
                case "Escape":
                    key = KeyCode.Escape;
                    break;
                case "Space":
                    key = KeyCode.Space;
                    break;
                case "Keypad0":
                    key = KeyCode.Keypad0;
                    break;
                case "Keypad1":
                    key = KeyCode.Keypad1;
                    break;
                case "Keypad2":
                    key = KeyCode.Keypad2;
                    break;
                case "Keypad3":
                    key = KeyCode.Keypad3;
                    break;
                case "Keypad4":
                    key = KeyCode.Keypad4;
                    break;
                case "Keypad5":
                    key = KeyCode.Keypad5;
                    break;
                case "Keypad6":
                    key = KeyCode.Keypad6;
                    break;
                case "Keypad7":
                    key = KeyCode.Keypad7;
                    break;
                case "Keypad8":
                    key = KeyCode.Keypad8;
                    break;
                case "Keypad9":
                    key = KeyCode.Keypad9;
                    break;
                case "KeypadPeriod":
                    key = KeyCode.KeypadPeriod;
                    break;
                case "KeypadDivide":
                    key = KeyCode.KeypadDivide;
                    break;
                case "KeypadMultiply":
                    key = KeyCode.KeypadMultiply;
                    break;
                case "KeypadMinus":
                    key = KeyCode.KeypadMinus;
                    break;
                case "KeypadPlus":
                    key = KeyCode.KeypadPlus;
                    break;
                case "KeypadEnter":
                    key = KeyCode.KeypadEnter;
                    break;
                case "KeypadEquals":
                    key = KeyCode.KeypadEquals;
                    break;
                case "UpArrow":
                    key = KeyCode.UpArrow;
                    break;
                case "DownArrow":
                    key = KeyCode.DownArrow;
                    break;
                case "RightArrow":
                    key = KeyCode.RightArrow;
                    break;
                case "LeftArrow":
                    key = KeyCode.LeftArrow;
                    break;
                case "Insert":
                    key = KeyCode.Insert;
                    break;
                case "Home":
                    key = KeyCode.Home;
                    break;
                case "End":
                    key = KeyCode.End;
                    break;
                case "PageUp":
                    key = KeyCode.PageUp;
                    break;
                case "PageDown":
                    key = KeyCode.PageDown;
                    break;
                case "F1":
                    key = KeyCode.F1;
                    break;
                case "F2":
                    key = KeyCode.F2;
                    break;
                case "F3":
                    key = KeyCode.F3;
                    break;
                case "F4":
                    key = KeyCode.F4;
                    break;
                case "F5":
                    key = KeyCode.F5;
                    break;
                case "F6":
                    key = KeyCode.F6;
                    break;
                case "F7":
                    key = KeyCode.F7;
                    break;
                case "F8":
                    key = KeyCode.F8;
                    break;
                case "F9":
                    key = KeyCode.F9;
                    break;
                case "F10":
                    key = KeyCode.F10;
                    break;
                case "F11":
                    key = KeyCode.F11;
                    break;
                case "F12":
                    key = KeyCode.F12;
                    break;
                case "F13":
                    key = KeyCode.F13;
                    break;
                case "F14":
                    key = KeyCode.F14;
                    break;
                case "F15":
                    key = KeyCode.F15;
                    break;
                case "Alpha0":
                    key = KeyCode.Alpha0;
                    break;
                case "Alpha1":
                    key = KeyCode.Alpha1;
                    break;
                case "Alpha2":
                    key = KeyCode.Alpha2;
                    break;
                case "Alpha3":
                    key = KeyCode.Alpha3;
                    break;
                case "Alpha4":
                    key = KeyCode.Alpha4;
                    break;
                case "Alpha5":
                    key = KeyCode.Alpha5;
                    break;
                case "Alpha6":
                    key = KeyCode.Alpha6;
                    break;
                case "Alpha7":
                    key = KeyCode.Alpha7;
                    break;
                case "Alpha8":
                    key = KeyCode.Alpha8;
                    break;
                case "Alpha9":
                    key = KeyCode.Alpha9;
                    break;
                case "Exclaim":
                    key = KeyCode.Exclaim;
                    break;
                case "DoubleQuote":
                    key = KeyCode.DoubleQuote;
                    break;
                case "Hash":
                    key = KeyCode.Hash;
                    break;
                case "Dollar":
                    key = KeyCode.Dollar;
                    break;
                case "Percent":
                    key = KeyCode.Percent;
                    break;
                case "Ampersand":
                    key = KeyCode.Ampersand;
                    break;
                case "Quote":
                    key = KeyCode.Quote;
                    break;
                case "LeftParen":
                    key = KeyCode.LeftParen;
                    break;
                case "RightParen":
                    key = KeyCode.RightParen;
                    break;
                case "Asterisk":
                    key = KeyCode.Asterisk;
                    break;
                case "Plus":
                    key = KeyCode.Plus;
                    break;
                case "Comma":
                    key = KeyCode.Comma;
                    break;
                case "Minus":
                    key = KeyCode.Minus;
                    break;
                case "Period":
                    key = KeyCode.Period;
                    break;
                case "Slash":
                    key = KeyCode.Slash;
                    break;
                case "Colon":
                    key = KeyCode.Colon;
                    break;
                case "Semicolon":
                    key = KeyCode.Semicolon;
                    break;
                case "Less":
                    key = KeyCode.Less;
                    break;
                case "Equals":
                    key = KeyCode.Equals;
                    break;
                case "Greater":
                    key = KeyCode.Greater;
                    break;
                case "Question":
                    key = KeyCode.Question;
                    break;
                case "At":
                    key = KeyCode.At;
                    break;
                case "LeftBracket":
                    key = KeyCode.LeftBracket;
                    break;
                case "Backslash":
                    key = KeyCode.Backslash;
                    break;
                case "RightBracket":
                    key = KeyCode.RightBracket;
                    break;
                case "Caret":
                    key = KeyCode.Caret;
                    break;
                case "Underscore":
                    key = KeyCode.Underscore;
                    break;
                case "BackQuote":
                    key = KeyCode.BackQuote;
                    break;
                case "A":
                    key = KeyCode.A;
                    break;
                case "B":
                    key = KeyCode.B;
                    break;
                case "C":
                    key = KeyCode.C;
                    break;
                case "D":
                    key = KeyCode.D;
                    break;
                case "E":
                    key = KeyCode.E;
                    break;
                case "F":
                    key = KeyCode.F;
                    break;
                case "G":
                    key = KeyCode.G;
                    break;
                case "H":
                    key = KeyCode.H;
                    break;
                case "I":
                    key = KeyCode.I;
                    break;
                case "J":
                    key = KeyCode.J;
                    break;
                case "K":
                    key = KeyCode.K;
                    break;
                case "L":
                    key = KeyCode.L;
                    break;
                case "M":
                    key = KeyCode.M;
                    break;
                case "N":
                    key = KeyCode.N;
                    break;
                case "O":
                    key = KeyCode.O;
                    break;
                case "P":
                    key = KeyCode.P;
                    break;
                case "Q":
                    key = KeyCode.Q;
                    break;
                case "R":
                    key = KeyCode.R;
                    break;
                case "S":
                    key = KeyCode.S;
                    break;
                case "T":
                    key = KeyCode.T;
                    break;
                case "U":
                    key = KeyCode.U;
                    break;
                case "V":
                    key = KeyCode.V;
                    break;
                case "W":
                    key = KeyCode.W;
                    break;
                case "X":
                    key = KeyCode.X;
                    break;
                case "Y":
                    key = KeyCode.Y;
                    break;
                case "Z":
                    key = KeyCode.Z;
                    break;
                case "LeftCurlyBracket":
                    key = KeyCode.LeftCurlyBracket;
                    break;
                case "Pipe":
                    key = KeyCode.Pipe;
                    break;
                case "RightCurlyBracket":
                    key = KeyCode.RightCurlyBracket;
                    break;
                case "Tilde":
                    key = KeyCode.Tilde;
                    break;
                case "Numlock":
                    key = KeyCode.Numlock;
                    break;
                case "CapsLock":
                    key = KeyCode.CapsLock;
                    break;
                case "ScrollLock":
                    key = KeyCode.ScrollLock;
                    break;
                case "RightShift":
                    key = KeyCode.RightShift;
                    break;
                case "LeftShift":
                    key = KeyCode.LeftShift;
                    break;
                case "RightControl":
                    key = KeyCode.RightControl;
                    break;
                case "LeftControl":
                    key = KeyCode.LeftControl;
                    break;
                case "RightAlt":
                    key = KeyCode.RightAlt;
                    break;
                case "LeftAlt":
                    key = KeyCode.LeftAlt;
                    break;
                case "LeftCommand":
                    key = KeyCode.LeftCommand;
                    break;
                case "LeftApple":
                    key = KeyCode.LeftApple;
                    break;
                case "LeftWindows":
                    key = KeyCode.LeftWindows;
                    break;
                case "RightCommand":
                    key = KeyCode.RightCommand;
                    break;
                case "RightApple":
                    key = KeyCode.RightApple;
                    break;
                case "RightWindows":
                    key = KeyCode.RightWindows;
                    break;
                case "AltGr":
                    key = KeyCode.AltGr;
                    break;
                case "Help":
                    key = KeyCode.Help;
                    break;
                case "Print":
                    key = KeyCode.Print;
                    break;
                case "SysReq":
                    key = KeyCode.SysReq;
                    break;
                case "Break":
                    key = KeyCode.Break;
                    break;
                case "Menu":
                    key = KeyCode.Menu;
                    break;
                case "Mouse0":
                    key = KeyCode.Mouse0;
                    break;
                case "Mouse1":
                    key = KeyCode.Mouse1;
                    break;
                case "Mouse2":
                    key = KeyCode.Mouse2;
                    break;
                case "Mouse3":
                    key = KeyCode.Mouse3;
                    break;
                case "Mouse4":
                    key = KeyCode.Mouse4;
                    break;
                case "Mouse5":
                    key = KeyCode.Mouse5;
                    break;
                case "Mouse6":
                    key = KeyCode.Mouse6;
                    break;

                case "JoystickButton0":
                    key = KeyCode.JoystickButton0;
                    break;
                case "JoystickButton1":
                    key = KeyCode.JoystickButton1;
                    break;
                case "JoystickButton2":
                    key = KeyCode.JoystickButton2;
                    break;
                case "JoystickButton3":
                    key = KeyCode.JoystickButton3;
                    break;
                case "JoystickButton4":
                    key = KeyCode.JoystickButton4;
                    break;
                case "JoystickButton5":
                    key = KeyCode.JoystickButton5;
                    break;
                case "JoystickButton6":
                    key = KeyCode.JoystickButton6;
                    break;
                case "JoystickButton7":
                    key = KeyCode.JoystickButton7;
                    break;
                case "JoystickButton8":
                    key = KeyCode.JoystickButton8;
                    break;
                case "JoystickButton9":
                    key = KeyCode.JoystickButton9;
                    break;
                case "JoystickButton10":
                    key = KeyCode.JoystickButton10;
                    break;
                case "JoystickButton11":
                    key = KeyCode.JoystickButton11;
                    break;
                case "JoystickButton12":
                    key = KeyCode.JoystickButton12;
                    break;
                case "JoystickButton13":
                    key = KeyCode.JoystickButton13;
                    break;
                case "JoystickButton14":
                    key = KeyCode.JoystickButton14;
                    break;
                case "JoystickButton15":
                    key = KeyCode.JoystickButton15;
                    break;
                case "JoystickButton16":
                    key = KeyCode.JoystickButton16;
                    break;
                case "JoystickButton17":
                    key = KeyCode.JoystickButton17;
                    break;
                case "JoystickButton18":
                    key = KeyCode.JoystickButton18;
                    break;
                case "JoystickButton19":
                    key = KeyCode.JoystickButton19;
                    break;

                case "Joystick1Button0":
                    key = KeyCode.Joystick1Button0;
                    break;
                case "Joystick1Button1":
                    key = KeyCode.Joystick1Button1;
                    break;
                case "Joystick1Button2":
                    key = KeyCode.Joystick1Button2;
                    break;
                case "Joystick1Button3":
                    key = KeyCode.Joystick1Button3;
                    break;
                case "Joystick1Button4":
                    key = KeyCode.Joystick1Button4;
                    break;
                case "Joystick1Button5":
                    key = KeyCode.Joystick1Button5;
                    break;
                case "Joystick1Button6":
                    key = KeyCode.Joystick1Button6;
                    break;
                case "Joystick1Button7":
                    key = KeyCode.Joystick1Button7;
                    break;
                case "Joystick1Button8":
                    key = KeyCode.Joystick1Button8;
                    break;
                case "Joystick1Button9":
                    key = KeyCode.Joystick1Button9;
                    break;
                case "Joystick1Button10":
                    key = KeyCode.Joystick1Button10;
                    break;
                case "Joystick1Button11":
                    key = KeyCode.Joystick1Button11;
                    break;
                case "Joystick1Button12":
                    key = KeyCode.Joystick1Button12;
                    break;
                case "Joystick1Button13":
                    key = KeyCode.Joystick1Button13;
                    break;
                case "Joystick1Button14":
                    key = KeyCode.Joystick1Button14;
                    break;
                case "Joystick1Button15":
                    key = KeyCode.Joystick1Button15;
                    break;
                case "Joystick1Button16":
                    key = KeyCode.Joystick1Button16;
                    break;
                case "Joystick1Button17":
                    key = KeyCode.Joystick1Button17;
                    break;
                case "Joystick1Button18":
                    key = KeyCode.Joystick1Button18;
                    break;
                case "Joystick1Button19":
                    key = KeyCode.Joystick1Button19;
                    break;

                case "Joystick2Button0":
                    key = KeyCode.Joystick2Button0;
                    break;
                case "Joystick2Button1":
                    key = KeyCode.Joystick2Button1;
                    break;
                case "Joystick2Button2":
                    key = KeyCode.Joystick2Button2;
                    break;
                case "Joystick2Button3":
                    key = KeyCode.Joystick2Button3;
                    break;
                case "Joystick2Button4":
                    key = KeyCode.Joystick2Button4;
                    break;
                case "Joystick2Button5":
                    key = KeyCode.Joystick2Button5;
                    break;
                case "Joystick2Button6":
                    key = KeyCode.Joystick2Button6;
                    break;
                case "Joystick2Button7":
                    key = KeyCode.Joystick2Button7;
                    break;
                case "Joystick2Button8":
                    key = KeyCode.Joystick2Button8;
                    break;
                case "Joystick2Button9":
                    key = KeyCode.Joystick2Button9;
                    break;
                case "Joystick2Button10":
                    key = KeyCode.Joystick2Button10;
                    break;
                case "Joystick2Button11":
                    key = KeyCode.Joystick2Button11;
                    break;
                case "Joystick2Button12":
                    key = KeyCode.Joystick2Button12;
                    break;
                case "Joystick2Button13":
                    key = KeyCode.Joystick2Button13;
                    break;
                case "Joystick2Button14":
                    key = KeyCode.Joystick2Button14;
                    break;
                case "Joystick2Button15":
                    key = KeyCode.Joystick2Button15;
                    break;
                case "Joystick2Button16":
                    key = KeyCode.Joystick2Button16;
                    break;
                case "Joystick2Button17":
                    key = KeyCode.Joystick2Button17;
                    break;
                case "Joystick2Button18":
                    key = KeyCode.Joystick2Button18;
                    break;
                case "Joystick2Button19":
                    key = KeyCode.Joystick2Button19;
                    break;

                case "Joystick3Button0":
                    key = KeyCode.Joystick3Button0;
                    break;
                case "Joystick3Button1":
                    key = KeyCode.Joystick3Button1;
                    break;
                case "Joystick3Button2":
                    key = KeyCode.Joystick3Button2;
                    break;
                case "Joystick3Button3":
                    key = KeyCode.Joystick3Button3;
                    break;
                case "Joystick3Button4":
                    key = KeyCode.Joystick3Button4;
                    break;
                case "Joystick3Button5":
                    key = KeyCode.Joystick3Button5;
                    break;
                case "Joystick3Button6":
                    key = KeyCode.Joystick3Button6;
                    break;
                case "Joystick3Button7":
                    key = KeyCode.Joystick3Button7;
                    break;
                case "Joystick3Button8":
                    key = KeyCode.Joystick3Button8;
                    break;
                case "Joystick3Button9":
                    key = KeyCode.Joystick3Button9;
                    break;
                case "Joystick3Button10":
                    key = KeyCode.Joystick3Button10;
                    break;
                case "Joystick3Button11":
                    key = KeyCode.Joystick3Button11;
                    break;
                case "Joystick3Button12":
                    key = KeyCode.Joystick3Button12;
                    break;
                case "Joystick3Button13":
                    key = KeyCode.Joystick3Button13;
                    break;
                case "Joystick3Button14":
                    key = KeyCode.Joystick3Button14;
                    break;
                case "Joystick3Button15":
                    key = KeyCode.Joystick3Button15;
                    break;
                case "Joystick3Button16":
                    key = KeyCode.Joystick3Button16;
                    break;
                case "Joystick3Button17":
                    key = KeyCode.Joystick3Button17;
                    break;
                case "Joystick3Button18":
                    key = KeyCode.Joystick3Button18;
                    break;
                case "Joystick3Button19":
                    key = KeyCode.Joystick3Button19;
                    break;

                case "Joystick4Button0":
                    key = KeyCode.Joystick4Button0;
                    break;
                case "Joystick4Button1":
                    key = KeyCode.Joystick4Button1;
                    break;
                case "Joystick4Button2":
                    key = KeyCode.Joystick4Button2;
                    break;
                case "Joystick4Button3":
                    key = KeyCode.Joystick4Button3;
                    break;
                case "Joystick4Button4":
                    key = KeyCode.Joystick4Button4;
                    break;
                case "Joystick4Button5":
                    key = KeyCode.Joystick4Button5;
                    break;
                case "Joystick4Button6":
                    key = KeyCode.Joystick4Button6;
                    break;
                case "Joystick4Button7":
                    key = KeyCode.Joystick4Button7;
                    break;
                case "Joystick4Button8":
                    key = KeyCode.Joystick4Button8;
                    break;
                case "Joystick4Button9":
                    key = KeyCode.Joystick4Button9;
                    break;
                case "Joystick4Button10":
                    key = KeyCode.Joystick4Button10;
                    break;
                case "Joystick4Button11":
                    key = KeyCode.Joystick4Button11;
                    break;
                case "Joystick4Button12":
                    key = KeyCode.Joystick4Button12;
                    break;
                case "Joystick4Button13":
                    key = KeyCode.Joystick4Button13;
                    break;
                case "Joystick4Button14":
                    key = KeyCode.Joystick4Button14;
                    break;
                case "Joystick4Button15":
                    key = KeyCode.Joystick4Button15;
                    break;
                case "Joystick4Button16":
                    key = KeyCode.Joystick4Button16;
                    break;
                case "Joystick4Button17":
                    key = KeyCode.Joystick4Button17;
                    break;
                case "Joystick4Button18":
                    key = KeyCode.Joystick4Button18;
                    break;
                case "Joystick4Button19":
                    key = KeyCode.Joystick4Button19;
                    break;

                case "Joystick5Button0":
                    key = KeyCode.Joystick5Button0;
                    break;
                case "Joystick5Button1":
                    key = KeyCode.Joystick5Button1;
                    break;
                case "Joystick5Button2":
                    key = KeyCode.Joystick5Button2;
                    break;
                case "Joystick5Button3":
                    key = KeyCode.Joystick5Button3;
                    break;
                case "Joystick5Button4":
                    key = KeyCode.Joystick5Button4;
                    break;
                case "Joystick5Button5":
                    key = KeyCode.Joystick5Button5;
                    break;
                case "Joystick5Button6":
                    key = KeyCode.Joystick5Button6;
                    break;
                case "Joystick5Button7":
                    key = KeyCode.Joystick5Button7;
                    break;
                case "Joystick5Button8":
                    key = KeyCode.Joystick5Button8;
                    break;
                case "Joystick5Button9":
                    key = KeyCode.Joystick5Button9;
                    break;
                case "Joystick5Button10":
                    key = KeyCode.Joystick5Button10;
                    break;
                case "Joystick5Button11":
                    key = KeyCode.Joystick5Button11;
                    break;
                case "Joystick5Button12":
                    key = KeyCode.Joystick5Button12;
                    break;
                case "Joystick5Button13":
                    key = KeyCode.Joystick5Button13;
                    break;
                case "Joystick5Button14":
                    key = KeyCode.Joystick5Button14;
                    break;
                case "Joystick5Button15":
                    key = KeyCode.Joystick5Button15;
                    break;
                case "Joystick5Button16":
                    key = KeyCode.Joystick5Button16;
                    break;
                case "Joystick5Button17":
                    key = KeyCode.Joystick5Button17;
                    break;
                case "Joystick5Button18":
                    key = KeyCode.Joystick5Button18;
                    break;
                case "Joystick5Button19":
                    key = KeyCode.Joystick5Button19;
                    break;

                case "Joystick6Button0":
                    key = KeyCode.Joystick6Button0;
                    break;
                case "Joystick6Button1":
                    key = KeyCode.Joystick6Button1;
                    break;
                case "Joystick6Button2":
                    key = KeyCode.Joystick6Button2;
                    break;
                case "Joystick6Button3":
                    key = KeyCode.Joystick6Button3;
                    break;
                case "Joystick6Button4":
                    key = KeyCode.Joystick6Button4;
                    break;
                case "Joystick6Button5":
                    key = KeyCode.Joystick6Button5;
                    break;
                case "Joystick6Button6":
                    key = KeyCode.Joystick6Button6;
                    break;
                case "Joystick6Button7":
                    key = KeyCode.Joystick6Button7;
                    break;
                case "Joystick6Button8":
                    key = KeyCode.Joystick6Button7;
                    break;
                case "Joystick6Button9":
                    key = KeyCode.Joystick6Button9;
                    break;
                case "Joystick6Button10":
                    key = KeyCode.Joystick6Button10;
                    break;
                case "Joystick6Button11":
                    key = KeyCode.Joystick6Button11;
                    break;
                case "Joystick6Button12":
                    key = KeyCode.Joystick6Button12;
                    break;
                case "Joystick6Button13":
                    key = KeyCode.Joystick6Button13;
                    break;
                case "Joystick6Button14":
                    key = KeyCode.Joystick6Button14;
                    break;
                case "Joystick6Button15":
                    key = KeyCode.Joystick6Button15;
                    break;
                case "Joystick6Button16":
                    key = KeyCode.Joystick6Button16;
                    break;
                case "Joystick6Button17":
                    key = KeyCode.Joystick6Button17;
                    break;
                case "Joystick6Button18":
                    key = KeyCode.Joystick6Button18;
                    break;
                case "Joystick6Button19":
                    key = KeyCode.Joystick6Button19;
                    break;

                case "Joystick7Button0":
                    key = KeyCode.Joystick7Button0;
                    break;
                case "Joystick7Button1":
                    key = KeyCode.Joystick7Button1;
                    break;
                case "Joystick7Button2":
                    key = KeyCode.Joystick7Button2;
                    break;
                case "Joystick7Button3":
                    key = KeyCode.Joystick7Button3;
                    break;
                case "Joystick7Button4":
                    key = KeyCode.Joystick7Button4;
                    break;
                case "Joystick7Button5":
                    key = KeyCode.Joystick7Button5;
                    break;
                case "Joystick7Button6":
                    key = KeyCode.Joystick7Button6;
                    break;
                case "Joystick7Button7":
                    key = KeyCode.Joystick7Button7;
                    break;
                case "Joystick7Button8":
                    key = KeyCode.Joystick7Button8;
                    break;
                case "Joystick7Button9":
                    key = KeyCode.Joystick7Button9;
                    break;
                case "Joystick7Button10":
                    key = KeyCode.Joystick7Button10;
                    break;
                case "Joystick7Button11":
                    key = KeyCode.Joystick7Button11;
                    break;
                case "Joystick7Button12":
                    key = KeyCode.Joystick7Button12;
                    break;
                case "Joystick7Button13":
                    key = KeyCode.Joystick7Button13;
                    break;
                case "Joystick7Button14":
                    key = KeyCode.Joystick7Button14;
                    break;
                case "Joystick7Button15":
                    key = KeyCode.Joystick7Button15;
                    break;
                case "Joystick7Button16":
                    key = KeyCode.Joystick7Button16;
                    break;
                case "Joystick7Button17":
                    key = KeyCode.Joystick7Button17;
                    break;
                case "Joystick7Button18":
                    key = KeyCode.Joystick7Button18;
                    break;
                case "Joystick7Button19":
                    key = KeyCode.Joystick7Button19;
                    break;

                case "Joystick8Button0":
                    key = KeyCode.Joystick8Button0;
                    break;
                case "Joystick8Button1":
                    key = KeyCode.Joystick8Button1;
                    break;
                case "Joystick8Button2":
                    key = KeyCode.Joystick8Button2;
                    break;
                case "Joystick8Button3":
                    key = KeyCode.Joystick8Button3;
                    break;
                case "Joystick8Button4":
                    key = KeyCode.Joystick8Button4;
                    break;
                case "Joystick8Button5":
                    key = KeyCode.Joystick8Button5;
                    break;
                case "Joystick8Button6":
                    key = KeyCode.Joystick8Button6;
                    break;
                case "Joystick8Button7":
                    key = KeyCode.Joystick8Button7;
                    break;
                case "Joystick8Button8":
                    key = KeyCode.Joystick8Button8;
                    break;
                case "Joystick8Button9":
                    key = KeyCode.Joystick8Button9;
                    break;
                case "Joystick8Button10":
                    key = KeyCode.Joystick8Button10;
                    break;
                case "Joystick8Button11":
                    key = KeyCode.Joystick8Button11;
                    break;
                case "Joystick8Button12":
                    key = KeyCode.Joystick8Button12;
                    break;
                case "Joystick8Button13":
                    key = KeyCode.Joystick8Button13;
                    break;
                case "Joystick8Button14":
                    key = KeyCode.Joystick8Button14;
                    break;
                case "Joystick8Button15":
                    key = KeyCode.Joystick8Button15;
                    break;
                case "Joystick8Button16":
                    key = KeyCode.Joystick8Button16;
                    break;
                case "Joystick8Button17":
                    key = KeyCode.Joystick8Button17;
                    break;
                case "Joystick8Button18":
                    key = KeyCode.Joystick8Button18;
                    break;
                case "Joystick8Button19":
                    key = KeyCode.Joystick8Button19;
                    break;
            }
            return key;
        }

        public static bool KeyDown(string s)
        {
            return Input.GetKeyDown(Key_Conversion(s));
        }

        public static bool KeyUp(string s)
        {
            return Input.GetKeyUp(Key_Conversion(s));
        }
        public static bool Key(string s)
        {
            return Input.GetKey(Key_Conversion(s));
        }
    }
    //ここまで


    //以下オプション設定関連
    public class Option_Data//設定データ
    {
        public float BGMvolume = 1;//BGM音量
        public float SEvolume = 1;//効果音音量
        public float Voicevolume = 1;//声音量
        public float RoteSpeed = 1;//声音量
        public int SelctoButton = 0;//選択ボタン 0:Xbox 1:PS4
    }

    public class OptionDataSystem//オプションロードセーブ
    {
        public static void Save(Option_Data data)//オプションセーブ
        {
            try
            {
                StreamWriter strw;
                string jos = JsonUtility.ToJson(data);
                strw = new StreamWriter(new FileStream(Application.dataPath + "/data/Option_Data.json", FileMode.Create));//ファイルがある場合上書き
                strw.Write(jos);
                strw.Flush();
                strw.Close();
            }
            catch
            {

                if (!Directory.Exists(Application.dataPath + "/data"))//フォルダーがある確認 ない場合フォルダー作成
                {
                    Debug.Log("保存先のフォルダーがないためフォルダーを作成します");
                    Directory.CreateDirectory(Application.dataPath + "/data");//フォルダー作成
                }
                Debug.Log("データを作成");
                StreamWriter strw;
                string jos = JsonUtility.ToJson(data);
                strw = new StreamWriter(new FileStream(Application.dataPath + "/data/Option_Data.json", FileMode.OpenOrCreate));
                strw.Write(jos);
                strw.Flush();
                strw.Close();
            }
        }

        public static Option_Data Lord()//オプションロード
        {
            try
            {
                StreamReader str;
                str = new StreamReader(new FileStream(Application.dataPath + "/data/Option_Data.json", FileMode.Open));
                string json = str.ReadToEnd();
                str.Close();
                return JsonUtility.FromJson<Option_Data>(json);
            }
            catch
            {
                Option_Data data = new Option_Data();
                Save(data);

                StreamReader str;
                str = new StreamReader(new FileStream(Application.dataPath + "/data/Option_Data.json", FileMode.Open));
                string json = str.ReadToEnd();
                str.Close();
                return JsonUtility.FromJson<Option_Data>(json);
            }
        }
    }
    public static class KeySelcotChange//選択キーの入れ替え
    {
        public static void KeyChange(int i)//決定キーの入れ替え　0：XBox標準 1:PS4系
        {
            StandaloneInputModule UI_InputSy = GameObject.Find("EventSystem").GetComponent<StandaloneInputModule>();

            switch (i)
            {
                case 0://標準
                    UI_InputSy.submitButton = "Submit";
                    UI_InputSy.cancelButton = "Cancel";
                    break;

                case 1://PS4
                    UI_InputSy.submitButton = "Submit_C";
                    UI_InputSy.cancelButton = "Cancel_C";
                    break;
            }
        }
    }
    //ここまで


    public static class KeyNameLog//入力されたボタンの名前を取得
    {
        public static string KyeName()
        {
            string retuns = "";
            if (Input.anyKeyDown)
            {
                foreach (KeyCode code in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(code))
                    {
                        // 入力されたキー名を表示
                        retuns = code.ToString();
                    }
                }
            }
            else retuns = "NotPushKey"; 
            return retuns;
        }
    }
}

public  class DataBase//プレイヤーの所持アイテムのデータ情報
{
    public (WeaponData WeponSetData, int WeponCounts) WeponDatas;//武器の詳細情報  武器の所持数
    public (ItemData ItemSetData, int ItemCounts) ItemDatas;//武アイテム詳細情報　アイテムの所持数
}
