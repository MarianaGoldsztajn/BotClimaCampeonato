using System;
using System.Net;
using Telegram.Bot;
using OpenWeatherMap;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;

namespace TelegramBot
{
    class Program
    {
        static TelegramBotClient telegramBot = new TelegramBotClient("1865326814:AAG7dLdFvtDWbeODRF_d41Q-UGvIuDLbRyU");
        static OpenWeatherMapClient OWM = new OpenWeatherMapClient("c8204d6fa61c168c63574d11620db8a1");
        static string response;
        static bool start = true;
        static bool first = true;
        static bool championshipFirstTime = true;
        static bool consultingChampionship;
        static bool consultingWeather;
        static bool weatherFirstTime = true;
        static int position = 0;
        static void Main(string[] args)
        {
            telegramBot.OnMessage += _botClient_OnMessage;
            telegramBot.StartReceiving();
            Console.ReadKey();
            telegramBot.StopReceiving();
        } 
        private async static void _botClient_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            {
                if (response != null)
                {
                    start = false;
                }
                response = e.Message.Text.ToUpper();
                if (first || response == "0")
                {
                    await telegramBot.SendTextMessageAsync(e.Message.Chat.Id, "Bienvenido!\nSeleccioná la opción que quieras consultar:\n1- Ver el clima de una ciudad.\n2- Ver las posiciones del campeonato.");
                    first = false;
                    consultingWeather = false;
                    consultingChampionship = false;
                    weatherFirstTime = true;
                    championshipFirstTime = true;
                }
                if (response == "1" || consultingWeather)
                {
                    consultingWeather = true;
                    if(weatherFirstTime)
                    {
                        weatherFirstTime = false;
                        await telegramBot.SendTextMessageAsync(e.Message.Chat.Id, "Ingresá el nombre de la ciudad que quieras consultar.\nPara volver al menú presione 0."); 
                    }
                    else {
                        if (response != "0")
                        {
                            CurrentWeatherResponse currentweather = null;
                            try
                            {
                                currentweather = await OWM.CurrentWeather.GetByName(response, language: OpenWeatherMapLanguage.SP);
                            }
                            catch (Exception ex)
                            {
                                await telegramBot.SendTextMessageAsync(e.Message.Chat.Id, "Ciudad no econtrada!\nIntenta nuevamente o presiona 0 volver al menú.");
                            }
                            if (currentweather != null)
                            {
                                string weather = currentweather.Weather.Value;
                                await telegramBot.SendTextMessageAsync(e.Message.Chat.Id, "El tiempo en " + response.ToLower()+ " está: " + weather + "!\nConsulta otra ciudad o presiona 0 para volver al menú.");
                            }
                        }
                    }
                }else if(response == "2" || consultingChampionship)
                {
                    string cc = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Usuario\Downloads\TelegramBot-20210709T125829Z-001\TelegramBot\TelegramBot\Database1.mdf;Integrated Security=True";
                    SqlConnection cn = new SqlConnection(cc);
                    consultingChampionship = true;
                    if (championshipFirstTime)
                    {
                        await telegramBot.SendTextMessageAsync(e.Message.Chat.Id, "Seleccione la opcion deseada: \nU- Para ver la posición de Uruguay.\nA- Para ver la posición de Argentina.\nB- Para ver la posición de Brasil.\nC- Para ver la posición de Colombia.\n0- Para volver al menú.");
                        championshipFirstTime = false;
                    }
                    else
                    {
                        Dictionary<string, string> dic = new Dictionary<string, string>() { { "U", "'URUGUAY'" }, { "A", "'ARGENTINA'" }, { "B", "'BRASIL'" }, { "C", "'COLOMBIA'" } };
                        if (dic.ContainsKey(response))
                        {
                            try
                            {
                                cn.Open();
                                string query = "SELECT * FROM POSITIONS WHERE TEAMNAME = " + dic[response];
                                SqlCommand cmd = new SqlCommand(query, cn);
                                SqlDataReader reader = cmd.ExecuteReader();
                                while (reader.Read())
                                {
                                    int score = reader.GetInt32(2);
                                    position = score;
                                }
                                await telegramBot.SendTextMessageAsync(e.Message.Chat.Id,"Esta en la posición: " + position + "!\nSeleccione otra opcion o presiona 0 para volver al menú");
                            }
                            catch (Exception ex)
                            {
                                await telegramBot.SendTextMessageAsync(e.Message.Chat.Id, "No pudimos procesar su solicitud.");
                            }
                        } else
                        {
                            await telegramBot.SendTextMessageAsync(e.Message.Chat.Id, "Opción no valida.\nIntenta nuevamente o presiona 0 volver al menú.");
                        }
                        cn.Close();
                    }  
                }
                else if(response != "2" && response != "1" && response != "0" && !start)
                {
                    await telegramBot.SendTextMessageAsync(e.Message.Chat.Id, "Opción no valida.\nMarque 0 para volver al menú!");
                }
            }
        }
    }
}
