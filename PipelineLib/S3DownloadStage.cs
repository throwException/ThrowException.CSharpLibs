using System;
using System.Collections.Generic;

namespace ThrowException.CSharpLibs.PipelineLib
{
    public class S3DownloadStage : ProcessStage
    {
        private const string AwsSecretAccessKeyEnvironmentVariable = "AWS_SECRET_ACCESS_KEY";
        private const string AwsAccessKeyIdEnvironmentVariable = "AWS_ACCESS_KEY_ID";

        private static string ConstructArguments(
            string endpointUrl,
            string bucket,
            string key)
        {
            return string.Format(
                "s3 --endpoint-url {0} cp s3://{1}/{2} -",
                endpointUrl,
                bucket,
                key);
        }

        public S3DownloadStage(
            IConfig config,
            string endpointUrl, 
            string awsAccessKeyId, 
            string awsSecretAccessKey,
            string bucket,
            string key)
            : base("s3-upload", config.AwsCliPath, ConstructArguments(endpointUrl, bucket, key))
        {
            WithEnvironment(AwsAccessKeyIdEnvironmentVariable, awsAccessKeyId);
            WithEnvironment(AwsSecretAccessKeyEnvironmentVariable, awsSecretAccessKey);
        }
    }
}
