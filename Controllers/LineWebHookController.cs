using Linebot_Iching.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace isRock.Template
{
    public class LineWebHookController : LineBot.LineWebHookControllerBase
    {
        private IWebHostEnvironment _hostEnvironment;

        public LineWebHookController(IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }

        [Route("api/LineBotWebHook")]
        [HttpPost]
        public IActionResult POST()
        {
            //"抓取LineUserID";
            var AdminUserId = ReceivedMessage.events[0].source.userId;
            // 資料來源
            string dataPath = $"{_hostEnvironment.ContentRootPath}\\Datas\\data.json";
            Iching64 iching = JsonConvert.DeserializeObject<Iching64>(System.IO.File.ReadAllText(dataPath));

            try
            {
                //設定ChannelAccessToken(從LineDeveloper網頁上獲取)
                ChannelAccessToken = "Token";
                
                //配合Line Verify
                if (ReceivedMessage.events == null || ReceivedMessage.events.Count <= 0 || ReceivedMessage.events.FirstOrDefault().replyToken == "00000000000000000000000000000000")
                {
                    return Ok();
                }

                //取得Line Event
                var LineEvent = ReceivedMessage.events.FirstOrDefault();
                var bot = new LineBot.Bot(ChannelAccessToken);
                var responseMsg = "";
                //準備回覆訊息
                if (LineEvent.type.ToLower() == "message" && LineEvent.message.type == "text")
                {
                    if (Regex.IsMatch(LineEvent.message.text, @"^\d{3}\s\d{3}\s\d{3}"))
                    {
                        #region 數字卦
                        var bottom = Convert.ToInt32(LineEvent.message.text.Substring(0, 3));
                        var top = Convert.ToInt32(LineEvent.message.text.Substring(4, 3));
                        var yao = Convert.ToInt32(LineEvent.message.text.Substring(8, 3));

                        var topTrigram = top % 8 == 0 ? 8 : top % 8;
                        var bottomTrigram = bottom % 8 == 0 ? 8 : bottom % 8;
                        var yaoX = yao % 6 == 0 ? 6 : yao % 6;

                        var result =  iching.hexagrams.Where(i => i.topTrigram == topTrigram && i.bottomTrigram == bottomTrigram).FirstOrDefault();

                        if(result != null)
                        {
                            var flexContents = @"
                                    {
                                      ""type"": ""bubble"",
                                      ""body"": {
                                        ""type"": ""box"",
                                        ""layout"": ""vertical"",
                                        ""contents"": [
                                          {
                                            ""type"": ""text"",
                                            ""text"": $name$,
                                            ""weight"": ""bold"",
                                            ""size"": ""xxl"",
                                            ""margin"": ""md""
                                          },
                                          {
                                            ""type"": ""text"",
                                            ""text"": $subname$,
                                            ""color"": ""#999999""
                                          },
                                          {
                                            ""type"": ""separator"",
                                            ""margin"": ""xxl""
                                          },
                                          {
                                            ""type"": ""box"",
                                            ""layout"": ""vertical"",
                                            ""margin"": ""xxl"",
                                            ""spacing"": ""sm"",
                                            ""contents"": [
                                              {
                                                ""type"": ""box"",
                                                ""layout"": ""vertical"",
                                                ""contents"": [
                                                  {
                                                    ""type"": ""spacer""
                                                  },
                                                  {
                                                    ""type"": ""text"",
                                                    ""text"": ""文言釋義"",
                                                    ""size"": ""sm"",
                                                    ""color"": ""#999999"",
                                                    ""flex"": 0,
                                                    ""wrap"": true
                                                  }
                                                ]
                                              },
                                              {
                                                ""type"": ""text"",
                                                ""text"": $wenyen$,
                                                ""size"": ""sm"",
                                                ""color"": ""#111111"",
                                                ""wrap"": true
                                              },
                                              {
                                                ""type"": ""box"",
                                                ""layout"": ""vertical"",
                                                ""contents"": [
                                                  {
                                                    ""type"": ""spacer""
                                                  },
                                                  {
                                                    ""type"": ""text"",
                                                    ""text"": ""白話釋義"",
                                                    ""size"": ""sm"",
                                                    ""color"": ""#999999"",
                                                    ""flex"": 0
                                                  }
                                                ]
                                              },
                                              {
                                                ""type"": ""text"",
                                                ""text"": $mandarin$,
                                                ""size"": ""sm"",
                                                ""color"": ""#111111"",
                                                ""wrap"": true
                                              }
                                            ]
                                          },
                                          {
                                            ""type"": ""separator"",
                                            ""margin"": ""xxl""
                                          },
                                          {
                                            ""type"": ""text"",
                                            ""text"": ""爻辭"",
                                            ""weight"": ""bold"",
                                            ""size"": ""lg"",
                                            ""margin"": ""md"",
                                            ""color"": ""#0000ff""
                                          }
                                          $yao$
                                        ]
                                      },

                                      ""footer"": {
                                        ""type"": ""box"",
                                        ""layout"": ""vertical"",
                                        ""spacing"": ""sm"",
                                        ""contents"": [
                                          {
                                            ""type"": ""button"",
                                            ""style"": ""link"",
                                            ""height"": ""sm"",
                                            ""action"": {
                                              ""type"": ""uri"",
                                              ""label"": ""易學網"",
                                              ""uri"": ""https://www.eee-learning.com/""
                                            }
                                          },
                                          {
                                            ""type"": ""spacer"",
                                            ""size"": ""sm""
                                          }
                                        ],
                                        ""flex"": 0
                                      }
                                    }";

                            string yauTXT = "";
                            var yaoItem = result.yao.Where(y => y.number == yaoX).FirstOrDefault();
                            if (yaoItem != null)
                            {
                                var res = @",{
                                                ""type"": ""text"",
                                                ""text"": $contents$,
                                                ""size"": ""md"",
                                                ""color"": ""#ff0000"",
                                                ""wrap"": true
                                              }";
                                res = res.Replace("$contents$", $@"""{yaoItem.name} {yaoItem.text}""");
                                yauTXT = res;
                            }

                            flexContents = flexContents.Replace("$name$", $@"""{result.name}""");
                            flexContents = flexContents.Replace("$subname$", $@"""{result.text}""");
                            flexContents = flexContents.Replace("$wenyen$", $@"""{String.Concat(result.description.wenyan)}""");
                            flexContents = flexContents.Replace("$mandarin$", $@"""{String.Concat(result.description.text)}""");
                            flexContents = flexContents.Replace("$yao$", yauTXT);

                            //定義一則訊息
                            var Messages = @"
                                    [
                                        {
                                            ""type"": ""flex"",
                                            ""altText"": $altText$,
                                            ""contents"": $flex$
                                        }
                                    ]";

                            //替換Flex Contents
                            var MessageJSON = Messages.Replace("$flex$", flexContents);
                            //發送訊息
                            bot.PushMessageWithJSON(AdminUserId, MessageJSON.Replace("$altText$", $@"""卦象: {result.name}"""));
                        }
                        else
                        {
                            responseMsg = $"數字卦: 請確認輸入數值";
                            ReplyMessage(LineEvent.replyToken, responseMsg);
                        }

                        #endregion
                    }
                    else if (Regex.IsMatch(LineEvent.message.text, @"^\d{2}") || Regex.IsMatch(LineEvent.message.text, @"^\d{1}"))
                    {
                        #region 卦序查詢
                        if (Convert.ToInt32(LineEvent.message.text) > 0 && Convert.ToInt32(LineEvent.message.text) < 65)
                        {
                            var result = iching.hexagrams.Where(i => i.number == Convert.ToInt32(LineEvent.message.text)).FirstOrDefault();
                            if (result != null)
                            {
                                var flexContents = @"
                                    {
                                      ""type"": ""bubble"",
                                      ""body"": {
                                        ""type"": ""box"",
                                        ""layout"": ""vertical"",
                                        ""contents"": [
                                          {
                                            ""type"": ""text"",
                                            ""text"": $name$,
                                            ""weight"": ""bold"",
                                            ""size"": ""xxl"",
                                            ""margin"": ""md""
                                          },
                                          {
                                            ""type"": ""text"",
                                            ""text"": $subname$,
                                            ""color"": ""#999999""
                                          },
                                          {
                                            ""type"": ""separator"",
                                            ""margin"": ""xxl""
                                          },
                                          {
                                            ""type"": ""box"",
                                            ""layout"": ""vertical"",
                                            ""margin"": ""xxl"",
                                            ""spacing"": ""sm"",
                                            ""contents"": [
                                              {
                                                ""type"": ""box"",
                                                ""layout"": ""vertical"",
                                                ""contents"": [
                                                  {
                                                    ""type"": ""spacer""
                                                  },
                                                  {
                                                    ""type"": ""text"",
                                                    ""text"": ""文言釋義"",
                                                    ""size"": ""sm"",
                                                    ""color"": ""#999999"",
                                                    ""flex"": 0,
                                                    ""wrap"": true
                                                  }
                                                ]
                                              },
                                              {
                                                ""type"": ""text"",
                                                ""text"": $wenyen$,
                                                ""size"": ""sm"",
                                                ""color"": ""#111111"",
                                                ""wrap"": true
                                              },
                                              {
                                                ""type"": ""box"",
                                                ""layout"": ""vertical"",
                                                ""contents"": [
                                                  {
                                                    ""type"": ""spacer""
                                                  },
                                                  {
                                                    ""type"": ""text"",
                                                    ""text"": ""白話釋義"",
                                                    ""size"": ""sm"",
                                                    ""color"": ""#999999"",
                                                    ""flex"": 0
                                                  }
                                                ]
                                              },
                                              {
                                                ""type"": ""text"",
                                                ""text"": $mandarin$,
                                                ""size"": ""sm"",
                                                ""color"": ""#111111"",
                                                ""wrap"": true
                                              }
                                            ]
                                          },
                                          {
                                            ""type"": ""separator"",
                                            ""margin"": ""xxl""
                                          },
                                          {
                                            ""type"": ""text"",
                                            ""text"": ""爻辭"",
                                            ""weight"": ""bold"",
                                            ""size"": ""lg"",
                                            ""margin"": ""md"",
                                            ""color"": ""#0000ff""
                                          }
                                          $yao$
                                        ]
                                      },

                                      ""footer"": {
                                        ""type"": ""box"",
                                        ""layout"": ""vertical"",
                                        ""spacing"": ""sm"",
                                        ""contents"": [
                                          {
                                            ""type"": ""button"",
                                            ""style"": ""link"",
                                            ""height"": ""sm"",
                                            ""action"": {
                                              ""type"": ""uri"",
                                              ""label"": ""易學網"",
                                              ""uri"": ""https://www.eee-learning.com/""
                                            }
                                          },
                                          {
                                            ""type"": ""spacer"",
                                            ""size"": ""sm""
                                          }
                                        ],
                                        ""flex"": 0
                                      }
                                    }";

                                string yauTXT = "";
                                foreach (var yao in result.yao)
                                {
                                    var res = @",{
                                            ""type"": ""text"",
                                            ""text"": $contents$,
                                            ""size"": ""md"",
                                            ""color"": ""#ff0000"",
                                            ""wrap"": true
                                        }";
                                    res = res.Replace("$contents$", $@"""{yao.name} {yao.text}""");
                                    yauTXT += res;
                                }

                                flexContents = flexContents.Replace("$name$", $@"""{result.name}""");
                                flexContents = flexContents.Replace("$subname$", $@"""{result.text}""");
                                flexContents = flexContents.Replace("$wenyen$", $@"""{String.Concat(result.description.wenyan)}""");
                                flexContents = flexContents.Replace("$mandarin$", $@"""{String.Concat(result.description.text)}""");
                                flexContents = flexContents.Replace("$yao$", yauTXT);

                                //定義一則訊息
                                var Messages = @"
                                    [
                                        {
                                            ""type"": ""flex"",
                                            ""altText"": $altText$,
                                            ""contents"": $flex$
                                        }
                                    ]";

                                //替換Flex Contents
                                var MessageJSON = Messages.Replace("$flex$", flexContents);
                                //發送訊息
                                bot.PushMessageWithJSON(AdminUserId, MessageJSON.Replace("$altText$", $@"""卦象: {result.name}"""));
                            }

                        }
                        else
                        {
                            responseMsg = $"查詢單一卦象，請輸入1-64之間整數";
                            ReplyMessage(LineEvent.replyToken, responseMsg);
                        }
                        #endregion
                    }
                    else
                    {
                        var flexContents = @"
                        {
                          ""type"": ""bubble"",
                          ""hero"": {
                            ""type"": ""image"",
                            ""url"": ""https://i.imgur.com/KfJUyG3.jpg"",
                            ""size"": ""full"",
                            ""aspectRatio"": ""20:13"",
                            ""aspectMode"": ""cover"",
                            ""action"": {
                              ""type"": ""uri"",
                              ""uri"": ""https://www.eee-learning.com/""
                            }
                          },
                          ""body"": {
                            ""type"": ""box"",
                            ""layout"": ""vertical"",
                            ""contents"": [
                              {
                                ""type"": ""text"",
                                ""text"": ""使用說明"",
                                ""weight"": ""bold"",
                                ""size"": ""xl""
                              },

                              {
                                ""type"": ""box"",
                                ""layout"": ""vertical"",
                                ""margin"": ""lg"",
                                ""spacing"": ""sm"",
                                ""contents"": [
                                  {
                                    ""type"": ""box"",
                                    ""layout"": ""baseline"",
                                    ""spacing"": ""sm"",
                                    ""contents"": [
                                      {
                                        ""type"": ""text"",
                                        ""text"": ""數字卦"",
                                        ""color"": ""#aaaaaa"",
                                        ""size"": ""sm"",
                                        ""flex"": 1
                                      },
                                      {
                                        ""type"": ""text"",
                                        ""text"": ""輸入三個數字，如 111 222 333"",
                                        ""wrap"": true,
                                        ""color"": ""#666666"",
                                        ""size"": ""sm"",
                                        ""flex"": 5
                                      }
                                    ]
                                  },
                                  {
                                    ""type"": ""box"",
                                    ""layout"": ""baseline"",
                                    ""spacing"": ""sm"",
                                    ""contents"": [
                                      {
                                        ""type"": ""text"",
                                        ""text"": ""查卦序"",
                                        ""color"": ""#aaaaaa"",
                                        ""size"": ""sm"",
                                        ""flex"": 1
                                      },
                                      {
                                        ""type"": ""text"",
                                        ""text"": ""輸入一個數字，範圍在1~64之間"",
                                        ""wrap"": true,
                                        ""color"": ""#666666"",
                                        ""size"": ""sm"",
                                        ""flex"": 5
                                      }
                                    ]
                                  }
                                ]
                              }
                            ]
                          },
                          ""footer"": {
                            ""type"": ""box"",
                            ""layout"": ""vertical"",
                            ""spacing"": ""sm"",
                            ""contents"": [
                              {
                                ""type"": ""button"",
                                ""style"": ""link"",
                                ""height"": ""sm"",
                                ""action"": {
                                  ""type"": ""uri"",
                                  ""label"": ""易學網"",
                                  ""uri"": ""https://www.eee-learning.com/""
                                }
                              },
                              {
                                ""type"": ""spacer"",
                                ""size"": ""sm""
                              }
                            ],
                            ""flex"": 0
                          }
                        }
                                ";
                        //定義一則訊息
                        var Messages = @"
                        [
                            {
                                ""type"": ""flex"",
                                ""altText"": ""This is a Flex Message"",
                                ""contents"": $flex$
                            }
                        ]";

                        //替換Flex Contents
                        var MessageJSON = Messages.Replace("$flex$", flexContents);
                        //發送訊息
                        bot.PushMessageWithJSON(AdminUserId, MessageJSON);
                    }

                }
                else if (LineEvent.type.ToLower() == "message")
                {
                    //responseMsg = $"收到 event : {LineEvent.type} type: {LineEvent.message.type} ";
                    responseMsg = $"尚未支援貼圖卜卦，請輸入正確文字內容";
                    //回覆訊息
                    ReplyMessage(LineEvent.replyToken, responseMsg);
                }
                else
                {
                    responseMsg = $"收到 event : {LineEvent.type} ";
                    //回覆訊息
                    ReplyMessage(LineEvent.replyToken, responseMsg);
                }
                
                return Ok();
            }
            catch (Exception ex)
            {
                //回覆訊息
                PushMessage(AdminUserId, "發生錯誤:\n" + ex.Message);
                //response OK
                return Ok();
            }
        }
    }
}