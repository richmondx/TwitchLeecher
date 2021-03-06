﻿using System;
using TwitchLeecher.Core.Enums;
using TwitchLeecher.Shared.Notification;

namespace TwitchLeecher.Core.Models
{
    public class SearchParameters : BindableBase
    {
        #region Fields

        private SearchType _searchType;
        private VideoType _videoType;

        private string _channel;
        private string _urls;
        private string _ids;

        private int _loadLimit;

        #endregion Fields

        #region Constructors

        public SearchParameters(SearchType searchType)
        {
            _searchType = searchType;
        }

        #endregion Constructors

        #region Properties

        public SearchType SearchType
        {
            get
            {
                return _searchType;
            }
            set
            {
                SetProperty(ref _searchType, value, nameof(SearchType));
            }
        }

        public VideoType VideoType
        {
            get
            {
                return _videoType;
            }
            set
            {
                SetProperty(ref _videoType, value, nameof(VideoType));
            }
        }

        public string Channel
        {
            get
            {
                return _channel;
            }
            set
            {
                SetProperty(ref _channel, value, nameof(Channel));
            }
        }

        public string Urls
        {
            get
            {
                return _urls;
            }
            set
            {
                SetProperty(ref _urls, value, nameof(Urls));
            }
        }

        public string Ids
        {
            get
            {
                return _ids;
            }
            set
            {
                SetProperty(ref _ids, value, nameof(Ids));
            }
        }

        public int LoadLimit
        {
            get
            {
                return _loadLimit;
            }
            set
            {
                SetProperty(ref _loadLimit, value, nameof(LoadLimit));
            }
        }

        #endregion Properties

        #region Methods

        public override void Validate(string propertyName = null)
        {
            base.Validate(propertyName);

            string currentProperty = nameof(Channel);

            if (string.IsNullOrWhiteSpace(propertyName) || propertyName == currentProperty)
            {
                if (_searchType == SearchType.Channel && string.IsNullOrWhiteSpace(_channel))
                {
                    AddError(currentProperty, "Please specify a channel name!");
                }
            }

            currentProperty = nameof(Urls);

            if (string.IsNullOrWhiteSpace(propertyName) || propertyName == currentProperty)
            {
                if (_searchType == SearchType.Urls)
                {
                    if (string.IsNullOrWhiteSpace(_urls))
                    {
                        AddError(currentProperty, "Please specify one or more Twitch video urls!");
                    }
                    else
                    {
                        Action addError = () =>
                        {
                            AddError(currentProperty, "One or more urls are invalid!");
                        };

                        string[] urls = _urls.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                        if (urls.Length > 0)
                        {
                            foreach (string url in urls)
                            {

                                if (!Uri.TryCreate(url, UriKind.Absolute, out Uri validUrl))
                                {
                                    addError();
                                    break;
                                }

                                string[] segments = validUrl.Segments;

                                if (segments.Length < 2)
                                {
                                    addError();
                                    break;
                                }

                                bool validId = false;

                                for (int i = 0; i < segments.Length; i++)
                                {
                                    if (segments[i].Equals("videos/", StringComparison.OrdinalIgnoreCase))
                                    {
                                        if (segments.Length > (i + 1))
                                        {
                                            string idStr = segments[i + 1];

                                            if (!string.IsNullOrWhiteSpace(idStr))
                                            {
                                                idStr = idStr.Trim(new char[] { '/' });


                                                if (int.TryParse(idStr, out int idInt) && idInt > 0)
                                                {
                                                    validId = true;
                                                    break;
                                                }
                                            }
                                        }

                                        break;
                                    }
                                }

                                if (!validId)
                                {
                                    addError();
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            currentProperty = nameof(Ids);

            if (string.IsNullOrWhiteSpace(propertyName) || propertyName == currentProperty)
            {
                if (_searchType == SearchType.Ids)
                {
                    if (string.IsNullOrWhiteSpace(_ids))
                    {
                        AddError(currentProperty, "Please specify one or more Twitch video IDs!");
                    }
                    else
                    {
                        string[] ids = _ids.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                        if (ids.Length > 0)
                        {
                            foreach (string id in ids)
                            {

                                if (!int.TryParse(id, out int idInt) || idInt <= 0)
                                {
                                    AddError(currentProperty, "One or more IDs are invalid!");
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        public SearchParameters Clone()
        {
            return new SearchParameters(_searchType)
            {
                VideoType = _videoType,
                Channel = _channel,
                Urls = _urls,
                Ids = _ids,
                LoadLimit = _loadLimit
            };
        }

        #endregion Methods
    }
}