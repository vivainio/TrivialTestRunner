from __future__ import print_function

import os,shutil

prjdir = "TrivialTestRunner"

def c(s):
    print(">",s)
    err = os.system(s)
    assert not err

shutil.rmtree(prjdir + "/bin")
shutil.rmtree(prjdir + "/obj")

os.chdir("Test")
c("dotnet run")

os.chdir("../" + prjdir)
c("dotnet pack -c Release")