using System;
using NUnit.Framework;
using System.Management;

namespace TestHost.FileSystemTests
{
    [TestFixture]
    public class FileSystemNavigationTests : FileSystemTestBase
    {
        [Test]
        public void CanSetLocationIntoSubDirectory()
        {
            var rootPath = base.SetupFileSystemWithStructure(new []{
                "/FolderA/SubFolderA/FileA"
            });

            var result = TestHost.ExecuteWithZeroErrors(
                "Set-Location " + rootPath,
                "Get-Location");
            
            result.Trim().ShouldEqual((string)rootPath);

            result = TestHost.ExecuteWithZeroErrors(
                "Set-Location " + rootPath + "/FolderA",
                "Get-Location");


            result.Trim().ShouldEqual((string)rootPath + "/FolderA");
            
            result = TestHost.ExecuteWithZeroErrors(
                "Set-Location " + rootPath + "/FolderA/SubfolderA",
                "Get-Location");
            
            result.Trim().ShouldEqual((string)rootPath + "/FolderA/SubfolderA");
            
        }

        [Test]
        [TestCase(".", "/a/b/c/d/e/f/g", "should not change directories")]
        [TestCase("./../../../../../../", "/a", "should navigate up a bunch of directories")]
        [TestCase("./..", "/a/b/c/d/e/f", "should nav up one dir")]
        [TestCase("./../", "/a/b/c/d/e/f", "should nav up one dir")]
        [TestCase("..", "/a/b/c/d/e/f", "should nav up one dir")]
        [TestCase("../", "/a/b/c/d/e/f", "should nav up one dir")]
        [TestCase("../g/h", "/a/b/c/d/e/f/g/h", "should nav up one dir and then back down 2 dirs")]
        [TestCase("../g/h/../../g/h", "/a/b/c/d/e/f/g/h", "should nav up one dir and then back down 2 dirs")]
        [TestCase("h", "/a/b/c/d/e/f/g/h", "should down one dir")]
        public void CDWithTwoPeriodsShouldMoveUpOneDirectory(string setLocationParam, string expectedLocation, string errorMessage)
        {
            var rootPath = base.SetupFileSystemWithStructure(new []{
                "/a/b/c/d/e/f/g/h/i/j/k/l/m/n/o/p"
            });

            var result = TestHost.ExecuteWithZeroErrors("Set-Location " + rootPath + "/a/b/c/d/e/f/g",
                                                        "Set-Location " + setLocationParam,
                                                        "Get-Location");
            
            result.Trim().ShouldEqual(rootPath + expectedLocation);
        }

        [Test]
        public void CDToInvalidDirectoryShouldThrowError()
        {
            var currentLocation = "Get-Location".Exec();

            var result = TestHost.ExecuteWithZeroErrors("set-location thisFolderReallyShouldNotExistNoReallyItShouldNotBeAnywhereOnAnyDiskAnywherePERIOD");
            result.ShouldContain("Cannot find path");
            result.ShouldContain("because it does not exist.");

            var currentLocationAfterBadCD = "Get-Location".Exec();

            currentLocation.ShouldEqual(currentLocationAfterBadCD);
        }
    }

    public static partial class _
    {
        public static string Exec(this string command)
        {
            var result = TestHost.ExecuteWithZeroErrors(command);
            if(result.EndsWith(Environment.NewLine))
            {
                // trim the new-line at the end
                return result.Substring(0, result.Length - Environment.NewLine.Length);
            }
            return result;
        }
    }
}

