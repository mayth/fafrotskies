fafrotskies
===========

ringing ringing terminal

# Requirements
.NET Framework 4 or later. This also works on Mono!

# Usage
```
fafrotskies [problem file]
```

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

# Fafrotskies?

＼　　／ 　 この幻想郷では常識に囚われては  
●　　●　　 いけないのですね！  
"　▽　"  
