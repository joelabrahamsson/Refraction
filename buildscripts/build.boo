solution_file = "Refraction.sln"
configuration = "release"
mspec_path = "packages/Machine.Specifications-Signed.0.4.13.0/tools/mspec.exe"
short_release_name = env("releasename")
release_name = "Refraction" + short_release_name
release_dir = "build"
release_versiondir = release_dir + "/" + release_name

target default, (compile):
  pass

target release, (compile, deploy, package):
  pass

desc "Compiles the solution"
target compile:
  msbuild(file: solution_file, configuration: configuration)

desc "Copies the binaries to the 'build' directory"
target deploy:
  print "Copying to build dir"

  with FileList("Refraction/bin/${configuration}"):
    .Include("*.{dll}")
    .ForEach def(file):
      file.CopyToDirectory(release_versiondir)

desc "Creates zip package"
target package:
  zip("build", release_dir + "/" + release_name  +'.zip')
  exec("git archive master --format=zip > ${release_dir}/${release_name}-src.zip")

