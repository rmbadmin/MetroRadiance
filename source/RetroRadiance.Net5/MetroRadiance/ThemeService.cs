using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using MetroRadiance.Internal;

#if NETCOREAPP
using Tavis.UriTemplates;
#endif

namespace MetroRadiance
{
	[Obsolete]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public enum Theme
	{
		Dark,
		Light,
	}

	[Obsolete]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public enum Accent
	{
		Purple,
		Blue,
		Orange,
		Original,
	}

	[Obsolete]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class ThemeService : INotifyPropertyChanged
	{
		#region singleton members
		public static ThemeService Current { get; } = new ThemeService();

		#endregion

		private static readonly string _baseUrl = @"pack://application:,,,/MetroRadiance;component/";
		private static readonly string _themeUrl = @"Themes/{theme}.xaml";
		private static readonly string _accentUrl = @"Themes/Accents/{accent}.xaml";
		private static readonly UriTemplateTable _templateTable;

		private static readonly UriTemplate _themeTemplate = new UriTemplate(_themeUrl);
		private static readonly UriTemplate _accentTemplate = new UriTemplate(_accentUrl);
		private static readonly Uri _templateBaseUri = new Uri(_baseUrl);

		private static readonly IReadOnlyDictionary<Accent, ResourceDictionary> accentDictionaries = new Dictionary<Accent, ResourceDictionary>
		{
			{ Accent.Blue, new ResourceDictionary { Source = CreateAccentResourceUri(Accent.Blue), } },
			{ Accent.Purple, new ResourceDictionary { Source = CreateAccentResourceUri(Accent.Purple), } },
			{ Accent.Orange, new ResourceDictionary { Source = CreateAccentResourceUri(Accent.Orange), } },
		};

		private bool initialized;
		private Dispatcher dispatcher;
		private ResourceDictionary currentAccentDictionary;

		private readonly List<ResourceDictionary> themeResources = new List<ResourceDictionary>();
		private readonly List<ResourceDictionary> accentResources = new List<ResourceDictionary>();

		#region Theme 変更通知プロパティ

		private Theme _Theme;

		public Theme Theme
		{
			get { return this._Theme; }
			private set
			{
				if (this._Theme != value)
				{
					this._Theme = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Accent 変更通知プロパティ

		private Accent _Accent;

		public Accent Accent
		{
			get { return this._Accent; }
			private set
			{
				if (this._Accent != value)
				{
					this._Accent = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion
		static ThemeService()
		{
#if NETCOREAPP
			_templateTable = new UriTemplateTable();
			_templateTable.Add("theme", new UriTemplate(_baseUrl + _themeUrl));
			_templateTable.Add("accent", new UriTemplate(_baseUrl + _accentUrl));
#else
			_templateTable = new UriTemplateTable(_templateBaseUri);
			_templateTable.KeyValuePairs.Add(new KeyValuePair<UriTemplate, Object>(_themeTemplate, "theme"));
			_templateTable.KeyValuePairs.Add(new KeyValuePair<UriTemplate, Object>(_accentTemplate, "accent"));
			_templateTable.MakeReadOnly(false);
#endif
		}

		private ThemeService() { }

		public void Initialize(Application app, Theme theme, Accent accent)
		{
			this.InitializeCore(app, theme, accentDictionaries[accent]);

			this.Theme = theme;
			this.Accent = accent;
		}

		public void Initialize(Application app, Theme theme, ResourceDictionary accent)
		{
			this.InitializeCore(app, theme, this.currentAccentDictionary);

			this.Theme = theme;
			this.Accent = Accent.Original;
		}

		private void InitializeCore(Application app, Theme theme, ResourceDictionary accent)
		{
			this.dispatcher = app.Dispatcher;

			this.currentAccentDictionary = accent;
			this.Register(app.Resources, theme, this.currentAccentDictionary);

			this.initialized = true;
		}

		public IDisposable Register(ResourceDictionary rd)
		{
			return this.Register(rd, this.Theme, this.currentAccentDictionary);
		}

		internal IDisposable Register(ResourceDictionary rd, Theme theme, ResourceDictionary accentDic)
		{
			var allDictionaries = EnumerateDictionaries(rd).ToArray();

			var themeDic = new ResourceDictionary { Source = CreateThemeResourceUri(theme), };
			var targetThemeDic = allDictionaries.FirstOrDefault(x => CheckThemeResourceUri(x.Source));
			if (targetThemeDic == null)
			{
				targetThemeDic = themeDic;
				rd.MergedDictionaries.Add(targetThemeDic);
			}
			else
			{
				foreach (var key in themeDic.Keys.OfType<string>().Where(x => targetThemeDic.Contains(x)))
				{
					targetThemeDic[key] = themeDic[key];
				}
			}
			this.themeResources.Add(targetThemeDic);

			var targetAccentDic = allDictionaries.FirstOrDefault(x => CheckAccentResourceUri(x.Source));
			if (targetAccentDic == null)
			{
				targetAccentDic = new ResourceDictionary { Source = accentDic.Source };
				rd.MergedDictionaries.Add(targetAccentDic);
			}
			else
			{
				foreach (var key in accentDic.Keys.OfType<string>().Where(x => targetAccentDic.Contains(x)))
				{
					targetAccentDic[key] = accentDic[key];
				}
			}
			this.accentResources.Add(targetAccentDic);

			// Unregister したいときは戻り値の IDisposable を Dispose() してほしい
			return Disposable.Create(() =>
			{
				this.themeResources.Remove(targetThemeDic);
				this.accentResources.Remove(targetAccentDic);
			});
		}

		public void ChangeTheme(Theme theme)
		{
			if (!this.initialized || this.Theme == theme) return;

			this.dispatcher.Invoke(() =>
			{
				var uri = CreateThemeResourceUri(theme);
				var dic = new ResourceDictionary { Source = uri, };

				foreach (var key in dic.Keys.OfType<string>())
				{
					foreach (var resource in this.themeResources.Where(x => x.Contains(key)))
					{
						resource[key] = dic[key];
					}
				}
			});

			this.Theme = theme;
		}

		public void ChangeAccent(Accent accent)
		{
			if (!this.initialized || this.Accent == accent) return;

			this.dispatcher.Invoke(() => this.ChangeAccentCore(accentDictionaries[accent]));
			this.Accent = accent;
		}

		public void ChangeAccent(ResourceDictionary accent)
		{
			if (!this.initialized) return;

			this.dispatcher.Invoke(() => this.ChangeAccentCore(accent));
			this.Accent = Accent.Original;
		}

		private void ChangeAccentCore(ResourceDictionary dic)
		{
			this.currentAccentDictionary = dic;
			foreach (var key in dic.Keys.OfType<string>())
			{
				foreach (var resource in this.accentResources.Where(x => x.Contains(key)))
				{
					resource[key] = dic[key];
				}
			}
		}

		static Uri CreateThemeResourceUri(Theme theme)
		{
#if NETCOREAPP
			var url = _themeTemplate
				.AddParameters(new
				{
					theme = theme.ToString()
				})
				.Resolve();
			return new Uri(_templateBaseUri, url);
#else
			var param = new Dictionary<string, string>
			{
				{ "theme", theme.ToString() },
			};
			return _themeTemplate.BindByName(_templateBaseUri, param);
#endif
		}

		static Uri CreateAccentResourceUri(Accent accent)
		{
#if NETCOREAPP
			var url = _accentTemplate
				.AddParameters(new
				{
					accent = accent.ToString()
				})
				.Resolve();
			return new Uri(_templateBaseUri, url);
#else
			var param = new Dictionary<string, string>
			{
				{ "accent", accent.ToString() },
			};
			return _accentTemplate.BindByName(_templateBaseUri, param);
#endif
		}

		/// <summary>
		/// 指定した <see cref="Uri"/> がテーマのリソースを指す URI かどうかをチェックします。
		/// </summary>
		/// <returns><paramref name="uri"/> がテーマのリソースを指す URI の場合は true、それ以外の場合は false。</returns>
		static bool CheckThemeResourceUri(Uri uri)
		{
			if (uri == null) return false;
#if NETCOREAPP
			return _templateTable.Match(uri)?.Key == "theme";
#else
#if false
			var result = _templateTable.Match(uri);
			return result != null && result.Count == 1 && result.First().Data.ToString() == "theme";
#else
			return _themeTemplate.Match(_templateBaseUri, uri) != null;
#endif
#endif
		}

		/// <summary>
		/// 指定した <see cref="Uri"/> がアクセント カラーのリソースを指す URI かどうかをチェックします。
		/// </summary>
		/// <returns><paramref name="uri"/> がアクセント カラーのリソースを指す URI の場合は true、それ以外の場合は false。</returns>
		static bool CheckAccentResourceUri(Uri uri)
		{
			if (uri == null) return false;
#if NETCOREAPP
			return _templateTable.Match(uri)?.Key == "accent";
#else
#if false
			var result = _templateTable.Match(uri);
			return result != null && result.Count == 1 && result.First().Data.ToString() == "accent";
#else
			return _accentTemplate.Match(_templateBaseUri, uri) != null;
#endif
#endif
		}

		private static IEnumerable<ResourceDictionary> EnumerateDictionaries(ResourceDictionary dictionary)
		{
			if (dictionary.MergedDictionaries.Count == 0)
			{
				yield break;
			}

			foreach (var mergedDictionary in dictionary.MergedDictionaries)
			{
				yield return mergedDictionary;

				foreach (var other in EnumerateDictionaries(mergedDictionary))
				{
					yield return other;
				}
			}
		}


		#region INotifyPropertyChanged 

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
		{
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion
	}
}
