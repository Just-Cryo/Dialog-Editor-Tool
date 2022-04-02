A Dialog Editor Tool created for A Cosmic Seed

Output is a text file that contains all of the data for the dialog tree visible in the editor 

File reads as follows:
The first character in each line specifies what kind of line it is for
Every parameter in each line is separated by a : - no leading or trailing, and the identifier is not separated by one
```
Identifiers:
! - new dialog entry
        trunk:id:branch
% - script
        domain:script:arg1:arg2:arg3...
$ - character portrait
        character:pose:side([L]eft/[C]enter/[R]ight):offset from edge %
# - voiceline
        character:voiceline:volume
~ - main dialog
        character:nameid (to be loaded from database): dialog text
* - additional values
        Speed:text speed - not written if speed is 100(%)
@ - begin conditional
        domain:variable:comparator(>/=/<):value
                values are handeled as floats
& - dialog option*
        trunk:counter:branch:option text**
= - next dialog entry, no options*
        trunk:counter:branch**
^ - jump to new entry [WHILE PROCESSING, the player cannot tell]
        trunk:counter:branch**
- - shorthand for Global.True = 1 conditional, used for default option if there is a conditional above. 

* these 2 options (&,=) can have a * after the identifier to indicate dialog is supposed to end, the entry is to be recorded for the next interaction.

** these 3 options, when specifying the dialog tag, can omit the trunk if it is identical to the current entry, omit the id if it is the current id + 1, and omit the branch if it is identical
```
Example dialog file:
[TestDialog.txt](https://github.com/Just-Cryo/Dialog-Editor-Tool/files/8403355/TestDialog.txt)

