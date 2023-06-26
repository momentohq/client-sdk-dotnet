using Amazon.S3;
using Amazon.S3.Model;

namespace MomentoRedisExampleLambdaHandler;

public class Utils
{
    private static IAmazonS3? _s3Client;
    private static readonly string BucketName = "my-super-secret-bucket";
    private static readonly string ObjectKey = "super-secret-source-data.json";
    
    public static async Task<string> GetSourceData()
    {
        string sourceData;
        try
        {
            // Create Amazon S3 Client
            _s3Client = new AmazonS3Client();
            GetObjectRequest request = new GetObjectRequest
            {
                BucketName = BucketName,
                Key = ObjectKey
            };

            using (GetObjectResponse response = await _s3Client.GetObjectAsync(request))
            {
                // Read the object content
                using (StreamReader reader = new StreamReader(response.ResponseStream))
                {
                    sourceData = await reader.ReadToEndAsync();
                }
            }

            return sourceData;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error reading from S3: " + ex.Message);
            throw;
        }
    }
}