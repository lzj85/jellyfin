﻿using MediaBrowser.Controller.Channels;
using MediaBrowser.Model.Channels;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Querying;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MediaBrowser.Api
{
    [Route("/Channels", "GET", Summary = "Gets available channels")]
    public class GetChannels : IReturn<QueryResult<BaseItemDto>>
    {
        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        /// <value>The user id.</value>
        [ApiMember(Name = "UserId", Description = "User Id", IsRequired = false, DataType = "string", ParameterType = "path", Verb = "GET")]
        public string UserId { get; set; }

        /// <summary>
        /// Skips over a given number of items within the results. Use for paging.
        /// </summary>
        /// <value>The start index.</value>
        [ApiMember(Name = "StartIndex", Description = "Optional. The record index to start at. All items with a lower index will be dropped from the results.", IsRequired = false, DataType = "int", ParameterType = "query", Verb = "GET")]
        public int? StartIndex { get; set; }

        /// <summary>
        /// The maximum number of items to return
        /// </summary>
        /// <value>The limit.</value>
        [ApiMember(Name = "Limit", Description = "Optional. The maximum number of records to return", IsRequired = false, DataType = "int", ParameterType = "query", Verb = "GET")]
        public int? Limit { get; set; }
    }

    [Route("/Channels/{Id}/Items", "GET", Summary = "Gets channel items")]
    public class GetChannelItems : IReturn<QueryResult<BaseItemDto>>
    {
        public string Id { get; set; }

        public string CategoryId { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        /// <value>The user id.</value>
        [ApiMember(Name = "UserId", Description = "User Id", IsRequired = false, DataType = "string", ParameterType = "path", Verb = "GET")]
        public string UserId { get; set; }

        /// <summary>
        /// Skips over a given number of items within the results. Use for paging.
        /// </summary>
        /// <value>The start index.</value>
        [ApiMember(Name = "StartIndex", Description = "Optional. The record index to start at. All items with a lower index will be dropped from the results.", IsRequired = false, DataType = "int", ParameterType = "query", Verb = "GET")]
        public int? StartIndex { get; set; }

        /// <summary>
        /// The maximum number of items to return
        /// </summary>
        /// <value>The limit.</value>
        [ApiMember(Name = "Limit", Description = "Optional. The maximum number of records to return", IsRequired = false, DataType = "int", ParameterType = "query", Verb = "GET")]
        public int? Limit { get; set; }

        [ApiMember(Name = "SortOrder", Description = "Sort Order - Ascending,Descending", IsRequired = false, DataType = "string", ParameterType = "query", Verb = "GET")]
        public SortOrder? SortOrder { get; set; }

        [ApiMember(Name = "Filters", Description = "Optional. Specify additional filters to apply. This allows multiple, comma delimeted. Options: IsFolder, IsNotFolder, IsUnplayed, IsPlayed, IsFavorite, IsRecentlyAdded, IsResumable, Likes, Dislikes", IsRequired = false, DataType = "string", ParameterType = "query", Verb = "GET", AllowMultiple = true)]
        public string Filters { get; set; }

        [ApiMember(Name = "SortBy", Description = "Optional. Specify one or more sort orders, comma delimeted. Options: Album, AlbumArtist, Artist, Budget, CommunityRating, CriticRating, DateCreated, DatePlayed, PlayCount, PremiereDate, ProductionYear, SortName, Random, Revenue, Runtime", IsRequired = false, DataType = "string", ParameterType = "query", Verb = "GET", AllowMultiple = true)]
        public string SortBy { get; set; }

        /// <summary>
        /// Gets the filters.
        /// </summary>
        /// <returns>IEnumerable{ItemFilter}.</returns>
        public IEnumerable<ItemFilter> GetFilters()
        {
            var val = Filters;

            if (string.IsNullOrEmpty(val))
            {
                return new ItemFilter[] { };
            }

            return val.Split(',').Select(v => (ItemFilter)Enum.Parse(typeof(ItemFilter), v, true));
        }
    }

    public class ChannelService : BaseApiService
    {
        private readonly IChannelManager _channelManager;

        public ChannelService(IChannelManager channelManager)
        {
            _channelManager = channelManager;
        }

        public object Get(GetChannels request)
        {
            var result = _channelManager.GetChannels(new ChannelQuery
            {
                Limit = request.Limit,
                StartIndex = request.StartIndex,
                UserId = request.UserId,

            }, CancellationToken.None).Result;

            return ToOptimizedResult(result);
        }

        public object Get(GetChannelItems request)
        {
            var result = _channelManager.GetChannelItems(new ChannelItemQuery
            {
                Limit = request.Limit,
                StartIndex = request.StartIndex,
                UserId = request.UserId,
                ChannelId = request.Id,
                CategoryId = request.CategoryId,
                SortOrder = request.SortOrder,
                SortBy = (request.SortBy ?? string.Empty).Split(',').Where(i => !string.IsNullOrWhiteSpace(i)).ToArray(),
                Filters = request.GetFilters().ToArray()

            }, CancellationToken.None).Result;

            return ToOptimizedResult(result);
        }
    }
}