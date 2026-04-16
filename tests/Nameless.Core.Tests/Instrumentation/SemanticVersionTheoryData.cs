namespace Nameless.Instrumentation;

public class SemanticVersionTheoryData : TheoryData<string, Version> {
    public SemanticVersionTheoryData() {
        Add("1.0.1", new Version("1.0.1"));
        Add("2.1.3-alpha", new Version("2.1.3"));
        Add("1.0.1-alpha.1", new Version("1.0.1"));
        Add("1.0.1+build.5", new Version("1.0.1"));
        Add("1.0.1-alpha+001", new Version("1.0.1"));
        Add("1.2.3", new Version("1.2.3"));
        Add("2.10.4-alpha.1", new Version("2.10.4"));
        Add("3.5.0-beta.11+build.42", new Version("3.5.0"));
        Add("10.4.7-alpha", new Version("10.4.7"));
        Add("2.15.0+build.22", new Version("2.15.0"));
        Add("3.1.8-beta.2+exp.sha", new Version("3.1.8"));

        Add("v1.0.1", new Version("1.0.1"));
        Add("v2.1.3-alpha", new Version("2.1.3"));
        Add("v1.0.1-alpha.1", new Version("1.0.1"));
        Add("v1.0.1+build.5", new Version("1.0.1"));
        Add("v1.0.1-alpha+001", new Version("1.0.1"));
        Add("v1.2.3", new Version("1.2.3"));
        Add("v2.10.4-alpha.1", new Version("2.10.4"));
        Add("v3.5.0-beta.11+build.42", new Version("3.5.0"));
        Add("v10.4.7-alpha", new Version("10.4.7"));
        Add("v2.15.0+build.22", new Version("2.15.0"));
        Add("v3.1.8-beta.2+exp.sha", new Version("3.1.8"));
    }
}