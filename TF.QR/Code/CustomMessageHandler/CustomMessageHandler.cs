namespace Senparc.Weixin.MP.Sample.CommonService.CustomMessageHandler
{
    using Senparc.Weixin.HttpUtility;
    using Senparc.Weixin.MP;
    using Senparc.Weixin.MP.AdvancedAPIs;
    using Senparc.Weixin.MP.AdvancedAPIs.AutoReply;
    using Senparc.Weixin.MP.AdvancedAPIs.Media;
    using Senparc.Weixin.MP.Containers;
    using Senparc.Weixin.MP.Entities;
    using Senparc.Weixin.MP.Entities.Request;
    using Senparc.Weixin.MP.MessageHandlers;
    using Senparc.Weixin.MP.Sample.CommonService.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Web.Configuration;

    public class CustomMessageHandler : MessageHandler<CustomMessageContext>
    {
        private string agentToken;
        private string agentUrl;
        private string appId;
        private string appSecret;
        private GetCurrentAutoreplyInfoResult rules;
        public static Dictionary<string, string> TemplateMessageCollection = new Dictionary<string, string>();
        private string wiweihiKey;

        public CustomMessageHandler(RequestMessageBase requestMessage)
            : base(requestMessage, null, 0, null)
        {
            this.agentUrl = WebConfigurationManager.AppSettings["WeixinAgentUrl"];
            this.agentToken = WebConfigurationManager.AppSettings["WeixinAgentToken"];
            this.wiweihiKey = WebConfigurationManager.AppSettings["WeixinAgentWeiweihiKey"];
            this.appId = WebConfigurationManager.AppSettings["WeixinAppId"];
            this.appSecret = WebConfigurationManager.AppSettings["WeixinAppSecret"];
        }

        public CustomMessageHandler(Stream inputStream, PostModel postModel, int maxRecordCount = 0)
            : base(inputStream, postModel, maxRecordCount, null)
        {
            this.agentUrl = WebConfigurationManager.AppSettings["WeixinAgentUrl"];
            this.agentToken = WebConfigurationManager.AppSettings["WeixinAgentToken"];
            this.wiweihiKey = WebConfigurationManager.AppSettings["WeixinAgentWeiweihiKey"];
            this.appId = WebConfigurationManager.AppSettings["WeixinAppId"];
            this.appSecret = WebConfigurationManager.AppSettings["WeixinAppSecret"];
            this.WeixinContext.ExpireMinutes = 3.0;
            if (!string.IsNullOrEmpty(postModel.AppId))
            {
                this.appId = postModel.AppId;
            }
            base.OmitRepeatedMessageFunc = delegate(IRequestMessageBase requestMessage)
            {
                RequestMessageText text = requestMessage as RequestMessageText;
                if ((text != null) && (text.Content == "容错"))
                {
                    return false;
                }
                return true;
            };
        }

        public IResponseMessageBase AutoReply(RequestMessageText requestMessage)
        {
            IResponseMessageBase base2 = null;
            try
            {
                string content = requestMessage.Content;
                if (this.rules == null)
                {
                    return base2;
                }
                foreach (KeywordAutoReplyInfo_Item item in this.rules.keyword_autoreply_info.list)
                {
                    ReplyListInfoItem item3;
                    ResponseMessageNews news;
                    bool flag = false;
                    foreach (KeywordListInfoItem item2 in item.keyword_list_info)
                    {
                        if (((item2.match_mode == AutoReplyMatchMode.equal) && (content == item2.content)) || ((item2.match_mode == AutoReplyMatchMode.contain) && content.Contains(item2.content)))
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (flag)
                    {
                        item3 = item.reply_list_info[0];
                        switch (item3.type)
                        {
                            case AutoReplyType.text:
                                {
                                    ResponseMessageText text = base.CreateResponseMessage<ResponseMessageText>();
                                    text.Content = item3.content;
                                    base2 = text;
                                    break;
                                }
                            case AutoReplyType.news:
                                goto Label_00EA;
                        }
                    }
                    continue;
                Label_00EA:
                    news = base.CreateResponseMessage<ResponseMessageNews>();
                    foreach (NewsInfoItem item4 in item3.news_info.list)
                    {
                        Article article = new Article
                        {
                            Title = item4.title,
                            Description = item4.digest,
                            PicUrl = item4.cover_url,
                            Url = item4.content_url
                        };
                        news.Articles.Add(article);
                    }
                    base2 = news;
                }
                if ((base2 == null) && !string.IsNullOrEmpty(this.rules.message_default_autoreply_info.content))
                {
                    ResponseMessageText text2 = base.CreateResponseMessage<ResponseMessageText>();
                    text2.Content = this.rules.message_default_autoreply_info.content;
                    base2 = text2;
                }
            }
            catch
            {
            }
            return base2;
        }

        public override IResponseMessageBase DefaultResponseMessage(IRequestMessageBase requestMessage)
        {
            ResponseMessageText text = base.CreateResponseMessage<ResponseMessageText>();
            text.Content = "这条消息来自DefaultResponseMessage。";
            return text;
        }

        public ResponseMessageNews GetUrlResponse(string openid)
        {
            ResponseMessageNews news = base.CreateResponseMessage<ResponseMessageNews>();
            Article item = new Article
            {
                Title = "黄岩公安网上办事大厅",
                Description = "黄岩公安网上办事大厅",
                PicUrl = ConfigurationManager.AppSettings["HostUrl"] + "/plugins/lywjw/images/bg.jpg",
                Url = ConfigurationManager.AppSettings["HostUrl"] + "/WeiXin/R?openid=" + openid
            };
            news.Articles.Add(item);
            return news;
        }

        private string GetWelcomeInfo()
        {
            try
            {
                if (((this.rules != null) && (this.rules.is_add_friend_reply_open == 1)) && (this.rules.add_friend_autoreply_info.type == AutoReplyType.text))
                {
                    return this.rules.add_friend_autoreply_info.content;
                }
            }
            catch
            {
            }
            return "欢迎关注";
        }

        public override IResponseMessageBase OnEvent_ClickRequest(RequestMessageEvent_Click requestMessage)
        {
            IResponseMessageBase base2 = null;
            switch (requestMessage.EventKey)
            {
                case "lobby":
                    return this.GetUrlResponse(requestMessage.FromUserName);

                case "OneClick":
                    {
                        ResponseMessageText text = base.CreateResponseMessage<ResponseMessageText>();
                        base2 = text;
                        text.Content = "您点击了底部按钮。\r\n为了测试微信软件换行bug的应对措施，这里做了一个——\r\n换行";
                        return base2;
                    }
                case "SubClickRoot_Text":
                    {
                        ResponseMessageText text2 = base.CreateResponseMessage<ResponseMessageText>();
                        base2 = text2;
                        text2.Content = "您点击了子菜单按钮。";
                        return base2;
                    }
                case "SubClickRoot_News":
                    {
                        ResponseMessageNews news = base.CreateResponseMessage<ResponseMessageNews>();
                        base2 = news;
                        Article item = new Article
                        {
                            Title = "您点击了子菜单图文按钮",
                            Description = "您点击了子菜单图文按钮，这是一条图文信息。",
                            PicUrl = "http://sdk.weixin.senparc.com/Images/qrcode.jpg",
                            Url = "http://sdk.weixin.senparc.com"
                        };
                        news.Articles.Add(item);
                        return base2;
                    }
                case "SubClickRoot_Music":
                    {
                        UploadTemporaryMediaResult result = MediaApi.UploadTemporaryMedia(AccessTokenContainer.TryGetAccessToken(this.appId, this.appSecret, false), UploadMediaFileType.thumb, Server.GetMapPath("~/Images/Logo.jpg"), 0x2710);
                        ResponseMessageMusic music = base.CreateResponseMessage<ResponseMessageMusic>();
                        base2 = music;
                        music.Music.Title = "天籁之音";
                        music.Music.Description = "真的是天籁之音";
                        music.Music.MusicUrl = "http://sdk.weixin.senparc.com/Content/music1.mp3";
                        music.Music.HQMusicUrl = "http://sdk.weixin.senparc.com/Content/music1.mp3";
                        music.Music.ThumbMediaId = result.thumb_media_id;
                        return base2;
                    }
                case "SubClickRoot_Image":
                    {
                        UploadTemporaryMediaResult result2 = MediaApi.UploadTemporaryMedia(AccessTokenContainer.TryGetAccessToken(this.appId, this.appSecret, false), UploadMediaFileType.image, Server.GetMapPath("~/Images/Logo.jpg"), 0x2710);
                        ResponseMessageImage image = base.CreateResponseMessage<ResponseMessageImage>();
                        base2 = image;
                        image.Image.MediaId = result2.media_id;
                        return base2;
                    }
                case "OAuth":
                    {
                        ResponseMessageNews news2 = base.CreateResponseMessage<ResponseMessageNews>();
                        Article article2 = new Article
                        {
                            Title = "OAuth2.0测试",
                            Description = "选择下面两种不同的方式进行测试，区别在于授权成功后，最后停留的页面。"
                        };
                        news2.Articles.Add(article2);
                        Article article3 = new Article
                        {
                            Title = "OAuth2.0测试（不带returnUrl），测试环境可使用",
                            Description = "OAuth2.0测试（不带returnUrl）",
                            Url = "http://sdk.weixin.senparc.com/oauth2",
                            PicUrl = "http://sdk.weixin.senparc.com/Images/qrcode.jpg"
                        };
                        news2.Articles.Add(article3);
                        string url = "/OAuth2/TestReturnUrl";
                        Article article4 = new Article
                        {
                            Title = "OAuth2.0测试（带returnUrl），生产环境强烈推荐使用",
                            Description = "OAuth2.0测试（带returnUrl）",
                            Url = "http://sdk.weixin.senparc.com/oauth2?returnUrl=" + url.UrlEncode(),
                            PicUrl = "http://sdk.weixin.senparc.com/Images/qrcode.jpg"
                        };
                        news2.Articles.Add(article4);
                        return news2;
                    }
                case "Description":
                    {
                        ResponseMessageText text3 = base.CreateResponseMessage<ResponseMessageText>();
                        text3.Content = this.GetWelcomeInfo();
                        return text3;
                    }
                case "SubClickRoot_PicPhotoOrAlbum":
                    {
                        ResponseMessageText text4 = base.CreateResponseMessage<ResponseMessageText>();
                        base2 = text4;
                        text4.Content = "您点击了【微信拍照】按钮。系统将会弹出拍照或者相册发图。";
                        return base2;
                    }
                case "SubClickRoot_ScancodePush":
                    {
                        ResponseMessageText text5 = base.CreateResponseMessage<ResponseMessageText>();
                        base2 = text5;
                        text5.Content = "您点击了【微信扫码】按钮。";
                        return base2;
                    }
                case "ConditionalMenu_Male":
                    {
                        ResponseMessageText text6 = base.CreateResponseMessage<ResponseMessageText>();
                        base2 = text6;
                        text6.Content = "您点击了个性化菜单按钮，您的微信性别设置为：男。";
                        return base2;
                    }
                case "ConditionalMenu_Femle":
                    {
                        ResponseMessageText text7 = base.CreateResponseMessage<ResponseMessageText>();
                        base2 = text7;
                        text7.Content = "您点击了个性化菜单按钮，您的微信性别设置为：女。";
                        return base2;
                    }
            }
            ResponseMessageText text8 = base.CreateResponseMessage<ResponseMessageText>();
            text8.Content = "您点击了按钮，EventKey：" + requestMessage.EventKey;
            return text8;
        }

        public override IResponseMessageBase OnEvent_SubscribeRequest(RequestMessageEvent_Subscribe requestMessage)
        {
            ResponseMessageText text = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);
            text.Content = this.GetWelcomeInfo();
            if (!string.IsNullOrEmpty(requestMessage.EventKey))
            {
                text.Content = text.Content + "\r\n============\r\n场景值：" + requestMessage.EventKey;
            }
            return text;
        }

        public override IResponseMessageBase OnEvent_UnsubscribeRequest(RequestMessageEvent_Unsubscribe requestMessage)
        {
            ResponseMessageText text = base.CreateResponseMessage<ResponseMessageText>();
            text.Content = "有空再来";
            return text;
        }

        public override IResponseMessageBase OnEventRequest(IRequestMessageEventBase requestMessage)
        {
            return base.OnEventRequest(requestMessage);
        }

        public override void OnExecuted()
        {
            base.OnExecuted();
        }

        public override void OnExecuting()
        {
            string accessTokenOrAppId = AccessTokenContainer.TryGetAccessToken(this.appId, this.appSecret, false);
            this.rules = AutoReplyApi.GetCurrentAutoreplyInfo(accessTokenOrAppId);
            if (this.CurrentMessageContext.StorageData == null)
            {
                this.CurrentMessageContext.StorageData = "";
            }
            base.OnExecuting();
        }

        public override IResponseMessageBase OnTextOrEventRequest(RequestMessageText requestMessage)
        {
            return null;
        }

        public override IResponseMessageBase OnTextRequest(RequestMessageText requestMessage)
        {
            base.CreateResponseMessage<ResponseMessageText>();
            string fromUserName = requestMessage.FromUserName;
            if (requestMessage.Content == "办事")
            {
                return this.GetUrlResponse(fromUserName);
            }
            return this.AutoReply(requestMessage);
        }
    }
}

