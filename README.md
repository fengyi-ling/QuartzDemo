# QuartzDemo
### Recommend Structure
see ./reprocess-architecture.jpg

### From demo to production Code
If need to apply to production code, still need to:
- 1.Should change domain type from string to Enum
- 2.Should use real repository and eventhub client
- 3.handle unhappy path, null reference situation
- 4.lock the job if another are execute, because both jobs read and write from reprocess table, 
or set a longer time gap to ensure that this two jobs will not trigger at the same time
- 5.misfire instruction, how should we do if one trigger is miss, we have two option: 
  - DoNothing, which is just wait for the next time (recommend, because it is a cron job, has the next trigger time)
  - FireOnceNow, fire at once


