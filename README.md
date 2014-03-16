fafrotskies
===========

ringing ringing terminal

# Requirements
.NET Framework 4 or later. This also works on Mono!

# Usage
```
fafrotskies [--port PORT] problem_file
```
`--port` is optional. The server will listen on port 3939/tcp by default.

# Problem File
It's YAML file.

```yaml
name: Problem Name
description: |
  Problem Description here!
  Let's study elementary mathemathics!
flag: FLAG{kogasachan_kawaii}
limit: 10
cases:
  -
    problem: 1+1=?
    answer: 2
  -
    problem: 3*3=?
    answer: 9
    limit: 15
```

## Time Limit
`limit` in the root hash is the default time limit, and `limit` in the cases hash is the problem specific time limit. In the above case, for case 1, the limit is 10 seconds, and for case 2, the limit is 15 seconds.

## telnet Sample
```
% telnet localhost 3939
Trying 127.0.0.1...
Connected to localhost.
Escape character is '^]'.
Welcome. We are the fafrotskies.

===== problem test =====
This is the test!

#1
here is the simple equation.
do you know... 1+1=?
2
#2
okay. Let's get to the next stage.
The next is: 2+2=?
4
Congratulations!
Flag is FLAG{123456789abcdef}
Connection closed by foreign host.
```

# Fafrotskies?

＼　　／ 　 この幻想郷では常識に囚われては  
●　　●　　 いけないのですね！  
"　▽　"  
