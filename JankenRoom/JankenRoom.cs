using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace JankenRoom
{
    //用語説明
    //socket()を使ってソケットを生成する。このとき、TCP通信に利用するプロトコルの種類を指定する。
    //bind()を使って、ソケットをローカルのアドレス（ソケットファイルやIP+ポート）にバインドする。
    //listen()を使って、ソケットに接続を待ち受けるように命令する。
    //accept()を使って、クライアントからの接続を受け付ける。
    //read() / write()を使って、データの送受信を行う。
    //close()を使って、ソケットを閉じる。
    //send()接続された Socket にデータを送信します。
    //GetBytes(String),GetBytes(Char[])
    //派生クラスでオーバーライドされた場合、指定した文字列に含まれるすべての文字をバイト シーケンスにエンコードします
    //エンコード＝データを別の形式に変換すること
    class S
    {
        public static void Main()
        {
            Console.WriteLine("JankenRoom");
            SocketServer();
            Console.ReadKey();
        }

        public static void SocketServer()
        {
            // IPアドレスやポートの設定
            byte[] bytes = new byte[1024];
            string hostName = Dns.GetHostName();
            IPHostEntry ipHostInfo = Dns.GetHostEntry(hostName);
            IPAddress ipAddress = ipHostInfo.AddressList[1];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

            // ソケットの作成
            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(localEndPoint);
            listener.Listen(10);

            Console.WriteLine("クライアントの接続を待っています...");

            // クライアント2つの接続を待つ
            //一つ目のクライアント
            Socket client1 = listener.Accept();
            Console.WriteLine("クライアント1が接続しました。");
            int bytesRec1 = client1.Receive(bytes);
            string playerName1 = Encoding.UTF8.GetString(bytes, 0, bytesRec1);
            Console.WriteLine($"{Encoding.UTF8.GetString(bytes, 0, bytesRec1)}");
            //数字だけを取り出す
            string charCount1 = Regex.Replace(playerName1, @"[^0-9]", "");

            //二つ目のクライアント
            Socket client2 = listener.Accept();
            Console.WriteLine("クライアント2が接続しました。");
            int bytesRec2 = client2.Receive(bytes);
            string playerName2 = Encoding.UTF8.GetString(bytes, 0, bytesRec2);
            Console.WriteLine($"{Encoding.UTF8.GetString(bytes, 0, bytesRec2)}");
            //数字だけを取り出す
            string charCount2 = Regex.Replace(playerName2, @"[^0-9]", "");

            //3つめのクライアント
            Socket client3 = listener.Accept();
            Console.WriteLine("クライアント2が接続しました。");
            int bytesRec3 = client3.Receive(bytes);
            string playerName3 = Encoding.UTF8.GetString(bytes, 0, bytesRec3);
            Console.WriteLine($"{Encoding.UTF8.GetString(bytes, 0, bytesRec3)}");
            //数字だけを取り出す
            string charCount3 = Regex.Replace(playerName3, @"[^0-9]", "");

            //Sendで送信している。
            //プレイヤーの名前数をもらってきてint型に変換
            //computerが理解できる言語に変換
            //相手側に送る
            byte[] comment1 = Encoding.UTF8.GetBytes(playerName2.Substring(7, int.Parse(charCount2)) + " と " + playerName3.Substring(7, int.Parse(charCount3)) + "　が勝負を挑んできた！");
            client1.Send(comment1);
            byte[] comment2 = Encoding.UTF8.GetBytes(playerName1.Substring(7, int.Parse(charCount1)) + " と " + playerName3.Substring(7, int.Parse(charCount3)) + "　が勝負を挑んできた！");
            client2.Send(comment2);
            byte[] comment3 = Encoding.UTF8.GetBytes(playerName1.Substring(7, int.Parse(charCount1)) + " と " + playerName2.Substring(7, int.Parse(charCount2)) + "　が勝負を挑んできた！");
            client3.Send(comment3);

            // クライアントにじゃんけんのメッセージを送信
            string sendData = "勝負！！じゃんけんゲーム！\r\n0:ぐう　1:ちょき　2:ぱあ\r\n";
            byte[] msg = Encoding.UTF8.GetBytes(sendData);
            client1.Send(msg);
            client2.Send(msg);
            client3.Send(msg);

            // クライアント1の手を受信
            bytesRec1 = client1.Receive(bytes);
            string client1HandStr = Encoding.UTF8.GetString(bytes, 0, bytesRec1);
            Console.WriteLine(playerName1.Substring(7, int.Parse(charCount1)) + "の手" + client1HandStr);

            // クライアント2の手を受信
            bytesRec2 = client2.Receive(bytes);
            string client2HandStr = Encoding.UTF8.GetString(bytes, 0, bytesRec2);
            Console.WriteLine(playerName2.Substring(7, int.Parse(charCount2)) + "の手" + client2HandStr);

            // クライアント3の手を受信
            bytesRec3 = client3.Receive(bytes);
            string client3HandStr = Encoding.UTF8.GetString(bytes, 0, bytesRec3);
            Console.WriteLine(playerName3.Substring(7, int.Parse(charCount3)) + "の手" + client3HandStr);

            // 勝敗の判定
            string result1 = "", result2 = "", result3 = "";
            if (int.TryParse(client1HandStr.Substring(0, 1), out int client1Hand) &&
                int.TryParse(client2HandStr.Substring(0, 1), out int client2Hand) &&
                int.TryParse(client3HandStr.Substring(0, 1), out int client3Hand))
            {
                //全員同じとき　または　全員違ったとき
                if ((client1Hand == client2Hand && client2Hand == client3Hand) ||
                    (client1Hand != client2Hand && client2Hand != client3Hand && client3Hand != client1Hand))
                {
                    result1 = result2 = result3 = "あいこ";
                }
                //2人が同じ手、1人だけ違う手のとき
                else if (client1Hand == client2Hand && client2Hand != client3Hand)
                {
                    // 1番目と2番目が勝ちか判定
                    if ((client1Hand + 1) % 3 == client3Hand)
                    {
                        result1 = result2 = "あなたの勝ち！";
                        result3 = "あなたの負け";
                    }
                    else
                    {
                        result1 = result2 = "あなたの負け";
                        result3 = "あなたの勝ち！";
                    }
                }
                else if (client1Hand == client3Hand && client3Hand != client2Hand)
                {
                    // 1番目と3番目が勝ちか判定
                    if ((client1Hand + 1) % 3 == client2Hand)
                    {
                        result1 = result3 = "あなたの勝ち！";
                        result2 = "あなたの負け";
                    }
                    else
                    {
                        result1 = result3 = "あなたの負け";
                        result2 = "あなたの勝ち！";
                    }
                }
                else
                {
                    // 2番目と3番目が勝ちか判定
                    if ((client2Hand + 1) % 3 == client1Hand)
                    {
                        result2 = result3 = "あなたの勝ち！";
                        result1 = "あなたの負け";
                    }
                    else
                    {
                        result2 = result3 = "あなたの負け";
                        result1 = "あなたの勝ち！";
                    }
                }
                //else
                //{
                //    int[] hands = new int[3] { client1Hand, client2Hand, client3Hand };
                //    string[] results = new string[3];

                //    for (int i = 0; i < 3; i++)
                //    {
                //        int next = (i + 1) % 3;
                //        int next2 = (i + 2) % 3;
                //        // 2人が同じ手、1人だけ違う手
                //        if (hands[next] == hands[next2] && hands[i] != hands[next])
                //        {
                //            // i番目が勝ちか判定
                //            if ((hands[i] + 1) % 3 == hands[next])
                //            {
                //                results[i] = "あなたの勝ち";
                //            }
                //            else
                //            {
                //                results[i] = "あなたの負け";
                //            }
                //        }
                //        else
                //        {
                //            results[i] = "あなたの勝ち";
                //            Console.WriteLine("明日バイト面戸");
                //        }
                //    }
                //    result1 = results[0];
                //    result2 = results[1];
                //    result3 = results[2];
                //}
                //else if ((client1Hand + 1) % 3 == client2Hand && client2Hand == client3Hand)
                //{
                //    result1 = "あなたの勝ち！";
                //    result2 = "あなたの負け";
                //    result3 = "あなたの負け";
                //}
                //else
                //{
                //    for()
                //    //あなたの勝ち＆あなたの負けと表示する
                //    result1 = "あなたの負け";
                //    result2 = "あなたの勝ち！";
                //    result3 = "誰だお前";
                //}
            }
            else//どの条件にも当てはまらないとき
            {
                result1 = result2 = result3 = "無効な手が入力されました。";
            }

            //// 結果をクライアントに送信
            client1.Send(Encoding.UTF8.GetBytes($"結果: {result1}\r\n"));
            client2.Send(Encoding.UTF8.GetBytes($"結果: {result2}\r\n"));
            client3.Send(Encoding.UTF8.GetBytes($"結果: {result3}\r\n"));

            // ソケットの終了
            client1.Shutdown(SocketShutdown.Both);
            client1.Close();
            client2.Shutdown(SocketShutdown.Both);
            client2.Close();
            client3.Shutdown(SocketShutdown.Both);
            client3.Close();
            listener.Close();
        }
    }
}
