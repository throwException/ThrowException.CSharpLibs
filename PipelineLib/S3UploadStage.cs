using System;
using System.Collections.Generic;

namespace ThrowException.CSharpLibs.PipelineLib
{
    public class S3UploadStage : ProcessStage
    {
        private const string AwsSecretAccessKeyEnvironmentVariable = "AWS_SECRET_ACCESS_KEY";
        private const string AwsAccessKeyIdEnvironmentVariable = "AWS_ACCESS_KEY_ID";

        private static string ConstructArguments(
            string endpointUrl,
            string bucket,
            string key,
            long expectedSize)
        {
            return string.Format(
                "s3 --endpoint-url {0} cp --expected-size {1} - s3://{2}/{3}",
                endpointUrl,
                expectedSize,
                bucket,
                key);
        }

        public S3UploadStage(
            IConfig config,
            string endpointUrl, 
            string awsAccessKeyId, 
            string awsSecretAccessKey,
            string bucket,
            string key,
            long expectedSize)
            : base("s3-upload", config.AwsCliPath, ConstructArguments(endpointUrl, bucket, key, expectedSize))
        {
            WithEnvironment(AwsAccessKeyIdEnvironmentVariable, awsAccessKeyId);
            WithEnvironment(AwsSecretAccessKeyEnvironmentVariable, awsSecretAccessKey);
        }
    }
}
