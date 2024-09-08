using RadarSearchOptimizely.Search.Models.Enums;

namespace RadarSearchOptimizely.Search.Models
{
    public sealed class RadarDictionary
    {
        #region Singleton

        private static volatile RadarDictionary _instance;
        private static readonly object SyncRoot = new object();

        public static RadarDictionary Instance
        {
            get
            {
                if (_instance != null) return _instance;
                lock (SyncRoot)
                {
                    if (_instance == null)
                        _instance = new RadarDictionary();
                }
                return _instance;
            }
        }

        #endregion

        #region Private Members

        private readonly Dictionary<string, string> _dictionary;
        private readonly Dictionary<char, char> _aclDictionary;

        #endregion

        #region Constructors

        private RadarDictionary()
        {
            _dictionary = new Dictionary<string, string>
            {
                {"or", Const.RadarwordOr},
                { "and", Const.RadarwordAnd },
                { "+", Const.RadarwordAnd },
                { "||", Const.RadarwordOr },
                { "&&", Const.RadarwordAnd }
            };

            _aclDictionary = new Dictionary<char, char>
            {
                { '-', Const.AclSpecial },
                { '+', Const.AclSpecial },
                { '&', Const.AclSpecial },
                { '|', Const.AclSpecial },
                { '!', Const.AclSpecial },
                { '(', Const.AclSpecial },
                { ')', Const.AclSpecial },
                { '{', Const.AclSpecial },
                { '}', Const.AclSpecial },
                { '[', Const.AclSpecial },
                { ']', Const.AclSpecial },
                { '^', Const.AclSpecial },
                { '"', Const.AclSpecial },
                { '~', Const.AclSpecial },
                { '*', Const.AclSpecial },
                { '?', Const.AclSpecial },
                { ':', Const.AclSpecial },
                { '\\', Const.AclSpecial },
                { ' ', Const.AclSpecial }
            };
        }

        #endregion

        #region Public methods

        public string GetKeyFromWord(string word)
        {
            try
            {
                var pair = _dictionary.FirstOrDefault(x => string.Equals(x.Value, word, StringComparison.CurrentCultureIgnoreCase));
                return pair.Key;
            }
            catch (Exception ex)
            {
                throw new Exception("RadarDictionary GetKeyFromWord", ex);
            }
        }

        public string GetValueFromKey(string key)
        {
            try
            {
                var pair = _dictionary.FirstOrDefault(x => string.Equals(x.Key, key, StringComparison.CurrentCultureIgnoreCase));
                return pair.Value;
            }
            catch (Exception ex)
            {
                throw new Exception("RadarDictionary GetValueFromKey", ex);
            }
        }

        public FilterReplacement FilterRadarReplacementText(string text)
        {
            var retval = new FilterReplacement();
            var isComplex = false;
            var split = text.Split(' ');
            for (var i = 0; i < split.Length; i++)
            {
                var word = GetValueFromKey(split[i].Trim());
                if (!string.IsNullOrEmpty(word))
                {
                    split[i] = word;
                    isComplex = true;
                }
            }

            retval.IsComplex = isComplex;
            retval.Text = string.Join(" ", split);
            return retval;
        }

        public string ReplaceAclChars(string input)
        {
            var list = input.ToCharArray();
            for (var i = 0; i < list.Length; i++)
            {
                char currentReplacement;
                if (_aclDictionary.TryGetValue(list[i], out currentReplacement))
                    list[i] = currentReplacement;
            }
            return new string(list).ToLower();
        }

        #endregion
    }

    public class FilterReplacement
    {
        public string Text { get; set; }
        public bool IsComplex { get; set; }

        public FilterReplacement()
        {
            Text = string.Empty;
            IsComplex = false;
        }
    }
}