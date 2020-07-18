﻿using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Blogifier.Core.Data
{
	public interface ICustomFieldRepository : IRepository<CustomField>
	{
		Task<BlogItem> GetBlogSettings();
		Task SaveBlogSettings(BlogItem blog);

		string GetCustomValue(string name);
		Task SaveCustomValue(string name, string value);

		Task<List<SocialField>> GetSocial(int authorId = 0);
		Task SaveSocial(SocialField socialField);
	}

	public class CustomFieldRepository : Repository<CustomField>, ICustomFieldRepository
	{
		AppDbContext _db;

		public CustomFieldRepository(AppDbContext db) : base(db)
		{
			_db = db;
		}

        #region Basic get/set

        public string GetCustomValue(string name)
		{
			var field = _db.CustomFields.Where(f => f.Name == name).FirstOrDefault();
			return field == null ? "" : field.Content;
		}

		public async Task SaveCustomValue(string name, string value)
		{
			var field = _db.CustomFields.Where(f => f.Name == name).FirstOrDefault();
			if (field == null)
			{
				_db.CustomFields.Add(new CustomField { Name = name, Content = value, AuthorId = 0 });
			}
			else
			{
				field.Content = value;
			}
			await _db.SaveChangesAsync();
		}

		#endregion

		#region Blog setttings

		public async Task<BlogItem> GetBlogSettings()
		{
			var blog = new BlogItem();
			CustomField title, desc, items, cover, logo, theme, culture, includefeatured, headerscript, footerscript;

			title = _db.CustomFields.Where(f => f.AuthorId == 0 && f.Name == Constants.BlogTitle).FirstOrDefault();
			desc = _db.CustomFields.Where(f => f.AuthorId == 0 && f.Name == Constants.BlogDescription).FirstOrDefault();
			items = _db.CustomFields.Where(f => f.AuthorId == 0 && f.Name == Constants.BlogItemsPerPage).FirstOrDefault();
			cover = _db.CustomFields.Where(f => f.AuthorId == 0 && f.Name == Constants.BlogCover).FirstOrDefault();
			logo = _db.CustomFields.Where(f => f.AuthorId == 0 && f.Name == Constants.BlogLogo).FirstOrDefault();
			theme = _db.CustomFields.Where(f => f.AuthorId == 0 && f.Name == Constants.BlogTheme).FirstOrDefault();
			culture = _db.CustomFields.Where(f => f.AuthorId == 0 && f.Name == Constants.Culture).FirstOrDefault();
			includefeatured = _db.CustomFields.Where(f => f.AuthorId == 0 && f.Name == Constants.IncludeFeatured).FirstOrDefault();
			headerscript = _db.CustomFields.Where(f => f.AuthorId == 0 && f.Name == Constants.HeaderScript).FirstOrDefault();
			footerscript = _db.CustomFields.Where(f => f.AuthorId == 0 && f.Name == Constants.FooterScript).FirstOrDefault();

			blog.Title = title == null ? "Something went wrong :/" : title.Content;
			blog.Description = desc == null ? "We are working to resolve the issue" : desc.Content;
			blog.ItemsPerPage = items == null ? 10 : int.Parse(items.Content);
			blog.Cover = cover == null ? "admin/img/cover.png" : cover.Content;
			blog.Logo = logo == null ? "admin/img/logo-white.png" : logo.Content;
			blog.Theme = theme == null ? "cleanblog" : theme.Content;
			blog.Culture = culture == null ? "en-US" : culture.Content;
			blog.IncludeFeatured = includefeatured == null ? false : bool.Parse(includefeatured.Content);
			blog.HeaderScript = headerscript == null ? "" : headerscript.Content;
			blog.FooterScript = footerscript == null ? "" : footerscript.Content;
			blog.SocialFields = await GetSocial();

			return await Task.FromResult(blog);
		}

		public async Task SaveBlogSettings(BlogItem blog)
		{
			var title = _db.CustomFields.Where(f => f.AuthorId == 0 && f.Name == Constants.BlogTitle).FirstOrDefault();
			var desc = _db.CustomFields.Where(f => f.AuthorId == 0 && f.Name == Constants.BlogDescription).FirstOrDefault();
			var items = _db.CustomFields.Where(f => f.AuthorId == 0 && f.Name == Constants.BlogItemsPerPage).FirstOrDefault();
			var cover = _db.CustomFields.Where(f => f.AuthorId == 0 && f.Name == Constants.BlogCover).FirstOrDefault();
			var logo = _db.CustomFields.Where(f => f.AuthorId == 0 && f.Name == Constants.BlogLogo).FirstOrDefault();
			var culture = _db.CustomFields.Where(f => f.AuthorId == 0 && f.Name == Constants.Culture).FirstOrDefault();
			var theme = _db.CustomFields.Where(f => f.AuthorId == 0 && f.Name == Constants.BlogTheme).FirstOrDefault();
			var includefeatured = _db.CustomFields.Where(f => f.AuthorId == 0 && f.Name == Constants.IncludeFeatured).FirstOrDefault();
			var headerscript = _db.CustomFields.Where(f => f.AuthorId == 0 && f.Name == Constants.HeaderScript).FirstOrDefault();
			var footerscript = _db.CustomFields.Where(f => f.AuthorId == 0 && f.Name == Constants.FooterScript).FirstOrDefault();

			if (title == null) _db.CustomFields.Add(new CustomField { AuthorId = 0, Name = Constants.BlogTitle, Content = blog.Title });
			else title.Content = blog.Title;

			if (desc == null) _db.CustomFields.Add(new CustomField { AuthorId = 0, Name = Constants.BlogDescription, Content = blog.Description });
			else desc.Content = blog.Description;

			if (items == null) _db.CustomFields.Add(new CustomField { AuthorId = 0, Name = Constants.BlogItemsPerPage, Content = blog.ItemsPerPage.ToString() });
			else items.Content = blog.ItemsPerPage.ToString();

			if (cover == null) _db.CustomFields.Add(new CustomField { AuthorId = 0, Name = Constants.BlogCover, Content = blog.Cover });
			else cover.Content = blog.Cover;

			if (logo == null) _db.CustomFields.Add(new CustomField { AuthorId = 0, Name = Constants.BlogLogo, Content = blog.Logo });
			else logo.Content = blog.Logo;

			if (culture == null) _db.CustomFields.Add(new CustomField { AuthorId = 0, Name = Constants.Culture, Content = blog.Culture });
			else culture.Content = blog.Culture;

			if (theme == null) _db.CustomFields.Add(new CustomField { AuthorId = 0, Name = Constants.BlogTheme, Content = blog.Theme });
			else theme.Content = blog.Theme;

			if (includefeatured == null) _db.CustomFields.Add(new CustomField { AuthorId = 0, Name = Constants.IncludeFeatured, Content = blog.IncludeFeatured.ToString() });
			else includefeatured.Content = blog.IncludeFeatured.ToString();

			if (headerscript == null) _db.CustomFields.Add(new CustomField { AuthorId = 0, Name = Constants.HeaderScript, Content = blog.HeaderScript });
			else headerscript.Content = blog.HeaderScript;

			if (footerscript == null) _db.CustomFields.Add(new CustomField { AuthorId = 0, Name = Constants.FooterScript, Content = blog.FooterScript });
			else footerscript.Content = blog.FooterScript;

			await _db.SaveChangesAsync();
		}

        #endregion

        #region Social fields

        /// <summary>
        /// This depends on convetion - custom fields must be saved in the common format
        /// For example: Name = "social|facebook|1" and Content = "http://your.facebook.page.com"
        /// </summary>
        /// <param name="authorId">Author ID or 0 if field is blog level</param>
        /// <returns>List of fields normally used to build social buttons in UI</returns>
        public async Task<List<SocialField>> GetSocial(int authorId = 0)
		{
			var socials = new List<SocialField>();
			var customFields = _db.CustomFields.Where(f => f.Name.StartsWith("social|") && f.AuthorId == authorId);

			if (customFields.Any())
			{
				foreach (CustomField field in customFields)
				{
					var fieldArray = field.Name.Split('|');
					if (fieldArray.Length > 2)
					{
						socials.Add(new SocialField
						{
							Title = fieldArray[1].Capitalize(),
							Icon = $"fa-{fieldArray[1]}",
							Rank = int.Parse(fieldArray[2]),
							Id = field.Id,
							Name = field.Name,
							AuthorId = field.AuthorId,
							Content = field.Content
						});
					}
				}
			}
			return await Task.FromResult(socials.OrderBy(s => s.Rank).ToList());
		}

		public async Task SaveSocial(SocialField socialField)
		{
			var field = _db.CustomFields.Where(f => f.AuthorId == socialField.AuthorId 
				&& f.Name.ToLower().StartsWith($"social|{socialField.Title.ToLower()}")).FirstOrDefault();

			if (field == null)
			{
				_db.CustomFields.Add(new CustomField { 
					Name = socialField.Name, 
					Content = socialField.Content, 
					AuthorId = socialField.AuthorId 
				});
			}
			else
			{
				field.Content = socialField.Content;
				field.Name = socialField.Name;
			}
			await _db.SaveChangesAsync();
		}

		#endregion
	}
}