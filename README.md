# `BlockBusterXaml` project  
This is just xaml based debugger for playtesting

![Screen](readme/screen.jpg?raw=true "Main window screenshot")


# `BlockBuster` project  
Actual tetris designed as class library that can be used anywhere. I plan to test it with Unity as well as xaml(XAML tested)  


## Features
- [Super Rotation System](https://tetris.fandom.com/wiki/SRS)
- 7-Bag spawn system
- Naive gravity
- Hard drops
- Wall kicks
- DAS  

## Field
It have 2 top rows to spawn tetrominoes that are intended to be invisible, they are visible in XAML project for debug though


# Tasks
## 🔥 High priority 
- [x] Implement SRS rotation
- [x] Implement gravity and speed levels
- [x] Ghost tetromino
- [x] Implement new cell type for playfield to be rendered, to it can have move properties
- [x] Game status event
- [x] Sound events
- [x] Implement wallkicks
- [x] Constrained(time/repetition) infinity(With toggle) https://tetris.fandom.com/wiki/Infinity
- [x] Fix windows spawn location
- [x] Make xaml next queue preview
- [x] Xaml hold preview
- [x] Xaml menu: start\restart\pause on esc
- [x] SpawnTetromino(): Implement 7 pack spawn system
- [x] Check performance
- [x] Create readme
- [x] Implement basic scoring
- [x] Settings class with defaults in txt file
- [x] Report actions to scoring counter
- [x] Implement debug playfield states
- [x] Check infinity again
- [x] Check that soft drop does not lock mino
- [x] Before assigning regular score I need to check if it is BTB, if so, then score BTB(actionScore*1.5) instead
- [x] Auto repeat rate for rotation\horizontal and vertical movement
- [x] 🔧 Check infinity, looks like after hard drop it waits for some time to spawn next mino and make sure that kick triggers infinity
- [x] 🔧 Clicking on 'New game' after another game started causes +50mb ram
- [x] 🔧 Level switched to 2 only after 20th mino
- [x] Delayed Auto Shift: the amount of time before an input is repeated/ARR is engaged.(Auto Repeat Rate)
- [x] 🔧 Do not DAS rotations and hard drops
- [ ] 🔧 Implement global game timer as a time reference for all time based conditions
- [ ] 🔧 On fast movement playfield update incorrectly, and some junk displayed until tile updated
- [ ] Do not call `UpdateFieldToDisplay()` if nothing changed
- [ ] 🔧 With DAS implementation input started to feel unresponsive for shifts, rotations are fine
- [ ] 🔧 S mino sometimes not locking in defalut rotation if left corner is not on foundation(Hard drop locks it though)
- [ ] 🔧 Soft drop should not lock mino
- [ ] 🔧 Soft drop counted towards score even if it doensn't dropped lines
- [ ] Implement scoring tests
- [ ] Tspin is not recognized by Score counter
- [ ] Save score to file with regard to game mode
- [ ] Game modes: Quick play\Endless\Marathon\Sprint\Ultra 
- [ ] Make replay funcionality: 
    - [ ] Check if engine determenistic somehow
    - [ ] Each frame(or more frequent) store game inputs
    - [ ] Create ReplaytInputHandler 
    - [ ] Create ReplayWriter, to write to stream(game inputs, tetromino order, timestamps delta, hold, hext, level, mode)

## ✨ Low priority
- [x] Figure out snake sequences and what to do with them in spawn queue (more than 2 snakes consequently not permited)
- [x] Icon for xaml project
- [x] Tests (maybe?)
- [x] Make more appropriate layout in xaml
- [x] Read about fourth kick upgrading simple move to diffucult move, so it won't break the combo and b2b counter
- [x] use Debug.WriteLineIf everywhere and look at config for toggle
- [ ] RotateLeftOrRight(): Make default rotation as part of kicks table so this methodd can be simplified
- [ ] Make XAML window resizable
- [ ] Move all basic moves (<0) to separate enum to use in playfield history
- [ ] In playfield history use Action.Landed instaed of mino property
- [ ] Check that T spin wil be rewarded if teromino not lands and disappears(contininues moving)
- [ ] Fix unit tests
- [ ] ObjectCopier: check obsolete code
- [ ] Fix spelling of 'tetromino' everywhere, or rebrand to Block
- [ ] Playtest variable levels up
- [ ] 20G does not have a ghost Tetrimino function for you. When you see the Tetrimino on the ground, it’ll be too late. But if your movement gets fast enough, you’ll get more time to look at the NEXTs. Remember,
- [ ] Refactor main class
- [ ] If I elect to implement ARE, then I should also add:
    - [ ] Initial hold system
    - [ ] Initial rotation system 
    - [ ] Charging for movements and for hold

## 💡 Ideas to check
- [ ] Make HistoryStack concurrent
- [ ] Probably I do too many checks and code can be simplified
- [ ] Recongnize different BTB's