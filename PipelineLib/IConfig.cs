using System;

namespace ThrowException.CSharpLibs.PipelineLib
{
    public interface IConfig
    {
        string OpensslPath { get; }
        string GzipPath { get; }
        string StreamAuthPath { get; }
        string AwsCliPath { get; }
        string BlockdeltaPath { get; }
    }
}
