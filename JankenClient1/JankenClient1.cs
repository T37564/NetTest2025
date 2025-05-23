using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml.Linq;

namespace JankenClient1
{
    class C
    {
        public static void Main()
        {
            Console.WriteLine("あなたのニックネームを入力してください。");
            string myName = Console.ReadLine();
            //改行
            Console.WriteLine();

            SocketClient(myName);
            Console.ReadKey();
        }


        public static void SocketClient(string playerName)
        {
            //IPアドレスやポートを設定(自PC、ポート:11000）
            string hostName = Dns.GetHostName();
            IPHostEntry ipHostInfo = Dns.GetHostEntry(hostName);
            IPAddress ipAddress = ipHostInfo.AddressList[1];
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

            //外部を指定する場合
            // IPAddress ipAddress = IPAddress.Parse("172.25.91.135");
            // IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

            //ソケットを作成
            Socket socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            //接続する。失敗するとエラーで落ちる。
            try
            {
                socket.Connect(remoteEP);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Connect Faild{e.ToString()}");
                return;
            }

            int nameCount = playerName.Length;
            //Sendで送信している。
            byte[] msg = Encoding.UTF8.GetBytes("Player名" + playerName + " <EOF>" + nameCount.ToString());
            socket.Send(msg);

            //Receiveで受信している。
            //誰が勝負を挑んできたかがわかる
            byte[] bytes1 = new byte[1024];
            int bytesRec1 = socket.Receive(bytes1);
            string data1 = Encoding.UTF8.GetString(bytes1, 0, bytesRec1);
            Console.WriteLine(data1);

            //改行
            Console.WriteLine();

            //Receiveで受信している。
            //何のゲームをするかを受け取る
            byte[] bytes2 = new byte[1024];
            int bytesRec2 = socket.Receive(bytes2);
            string data2 = Encoding.UTF8.GetString(bytes2, 0, bytesRec2);
            Console.WriteLine(data2);

            // 入力した文字列を送信する
            string userInput = Console.ReadLine();
            // 入力した文字列を送信
            msg = Encoding.UTF8.GetBytes(userInput + "<EOF>");
            socket.Send(msg);

            //Receiveで受信している。
            byte[] bytes3 = new byte[1024];
            int bytesRec3 = socket.Receive(bytes3);
            data1 = Encoding.UTF8.GetString(bytes3, 0, bytesRec3);
            Console.WriteLine(data1);



            //ソケットを終了している。
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }
    }
}