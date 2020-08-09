using GrooverAdm.DataAccess.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrooverAdm.DataAccess.Dao
{
    public interface IPlaylistDao<T> where T : Playlist
    {
        /// <summary>
        /// Creates a playlist
        /// </summary>
        /// <param name="playlist"></param>
        /// <returns>A playlist with the updated ID</returns>
        Task<T> CreatePlaylist(T playlist);
        /// <summary>
        /// Deletes a playlist.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>True if playlist was deleted</returns>
        Task<bool> DeletePlaylist(string id);
        /// <summary>
        /// Updates a playlist
        /// </summary>
        /// <param name="playlist"></param>
        /// <returns>The updated playlist</returns>
        Task<T> UpdatePlaylist(T playlist);
        /// <summary>
        /// Gets a playlist by id of the place
        /// </summary>
        /// <param name="place"></param>
        /// <returns>The corresponding playlist</returns>
        Task<T> GetPlaylist(string place);

    }
}
