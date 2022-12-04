# QuartzDemo
### Recommend Structure
see ./reprocess-architecture.jpg

### From demo to production Code
If need to apply to production code, still need to:
- 1.Should change domain type from string to Enum
- 2.Should use real repository and eventhub client
- 3.handle unhappy path, null reference situation
- 4.lock the job if another are execute, because both jobs read and write from reprocess table


