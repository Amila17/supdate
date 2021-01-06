using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Google.Apis.Json;
using Supdate.Model;

namespace Supdate.Business.DataSources
{
  public class GoogleOAuthDataStore : IGoogleOAuthDataStore
  {
    private readonly IExternalApiAuthManager _externalApiAuthManager;

    public GoogleOAuthDataStore(IExternalApiAuthManager externalApiAuthManager)
    {
      _externalApiAuthManager = externalApiAuthManager;
    }

    public Task StoreAsync<T>(string key, T value)
    {
      var companyGuid = ExtractCompanyGuid(key);
      var serialized = NewtonsoftJsonSerializer.Instance.Serialize(value);

      var typeParameterType = typeof(T);
      Debug.WriteLine("GETASYNC");
      Debug.WriteLine("Key is:" + key);
      Debug.WriteLine("T is:" + typeParameterType.FullName);
      Debug.WriteLine("value is:" + value);

      // Check the serialized data can be converted back,
      // if not then it wasn't a valid value and shouldn't be saved
      try
      {
        var check = NewtonsoftJsonSerializer.Instance.Deserialize<T>(serialized);
      }
      catch (Exception ex)
      {
        return Task.Delay(0);
      }

      var externalApiAuth = _externalApiAuthManager.GetByCompanyGuid(companyGuid, ExternalApi.GoogleAnalytics.Id);
      if (externalApiAuth == null)
      {
        // New User we insert it into the database
        externalApiAuth = new ExternalApiAuth { ExternalApiId = ExternalApi.GoogleAnalytics.Id };
        externalApiAuth = StoreValue(key, serialized, externalApiAuth);
        _externalApiAuthManager.SaveWithCompanyGuid(companyGuid, externalApiAuth);
      }
      else
      {
        // Existing User We update it
        externalApiAuth = StoreValue(key, serialized, externalApiAuth);
        _externalApiAuthManager.Update(externalApiAuth);
      }

      return Task.Delay(0);
    }

    /// <summary>
    /// Deletes the given key. It deletes the <see cref="GenerateStoredKey"/> named file in <see cref="FolderPath"/>.
    /// </summary>
    /// <param name="key">The key to delete from the data store</param>
    public Task DeleteAsync<T>(string key)
    {
      var companyGuid = ExtractCompanyGuid(key);
      var externalApiAuth = _externalApiAuthManager.GetByCompanyGuid(companyGuid, ExternalApi.GoogleAnalytics.Id);

      if (externalApiAuth != null)
      {
        if (key.StartsWith("oauth_"))
        {
          externalApiAuth.Key = string.Empty;
        }
        else
        {
          externalApiAuth.Token = string.Empty;
        }

        if (string.IsNullOrWhiteSpace(externalApiAuth.Key) && string.IsNullOrWhiteSpace(externalApiAuth.Token))
        {
          _externalApiAuthManager.Delete(externalApiAuth.Id);
        }
        else
        {
          _externalApiAuthManager.Update(externalApiAuth);
        }
      }

      return Task.Delay(0);
    }

    public Task<T> GetAsync<T>(string key)
    {
      var companyGuid = ExtractCompanyGuid(key);

      var tcs = new TaskCompletionSource<T>();

      var externalApiAuth = _externalApiAuthManager.GetByCompanyGuid(companyGuid, ExternalApi.GoogleAnalytics.Id);
      if (externalApiAuth == null)
      {
        // We don't have a record
        tcs.SetResult(default(T));
      }
      else
      {
        try
        {
          // We have one
          Type typeParameterType = typeof(T);
          Debug.WriteLine("GETASYNC");
          Debug.WriteLine("Key is:" + key);
          Debug.WriteLine("T is:" + typeParameterType.FullName);
          Debug.WriteLine("token is:" + externalApiAuth.Token);
          var value = key.StartsWith("oauth_") ? externalApiAuth.Key : externalApiAuth.Token;
          tcs.SetResult(NewtonsoftJsonSerializer.Instance.Deserialize<T>(value)); //RefreshToken
        }
        catch (Exception ex)
        {
          tcs.SetException(ex);
        }
      }

      return tcs.Task;
    }

    public Task ClearAsync()
    {
      // Meant to remove all data from the Table!.
      return Task.Delay(0);
    }

    private static ExternalApiAuth StoreValue(string key, string value, ExternalApiAuth externalApiAuth)
    {
      var useKeyField = key.StartsWith("oauth_");
      if (useKeyField)
      {
        externalApiAuth.Key = value;
      }
      else
      {
        externalApiAuth.Token = value;
      }
      return externalApiAuth;
    }

    private static Guid ExtractCompanyGuid(string key)
    {
      if (string.IsNullOrEmpty(key))
      {
        throw new ArgumentException("Key MUST have a value");
      }

      key = key.Replace("oauth_", "");

      var companyGuid = Guid.Empty;
      if (!Guid.TryParse(key, out companyGuid))
      {
        throw new ArgumentException(string.Format("Key ({0}) MUST be a Guid", key));
      }

      return companyGuid;
    }

  }
}
