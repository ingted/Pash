[![Linux Build Status (Travis-CI)](https://secure.travis-ci.org/Pash-Project/Pash.png?branch=master )](http://travis-ci.org/Pash-Project/Pash)
[![Windows Build Status (AppVeyor)](https://ci.appveyor.com/api/projects/status/w6027t7hoqblsvow/branch/master)](https://ci.appveyor.com/project/JayBazuzi/pash/branch/master)
[![Pull Request Stats](http://issuestats.com/github/Pash-Project/Pash/badge/pr)](http://issuestats.com/github/Pash-Project/Pash)

[Pash](https://github.com/Pash-Project/Pash/)
====
 add invoke ps1 script feature (-f)
====
 add invoke commands directly feature (-c)
====
root@cd1:~# Pash.exe -c \$a = psql "-p40001 -h '10.128.112.40' -Upostgres -c 'select 1 as no;'" \; \$a
$a = psql -p40001 -h '10.128.112.40' -Upostgres -c 'select 1 as no;' ; $a
 no
----
  1
(1 row)

