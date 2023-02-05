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
🔥 - High priority or MVP item  
🔧 - Bug  
✨ - Low priority\Polish  
💡 - Idea  

| New | Doing | Done |
|-----|-------|------|
|🔧🔥 Tspin is not recognized by Score counter|||
|🔧🔥 Check that combo can be achieved in gameplay|||
|🔧🔥 Implement scoring tests|||
|🔥 Save score to file with regard to game mode|||
|🔥 Game modes: Quick play\Endless\Marathon\Sprint\Ultra |||
|🔥 Make replay functionality: Check if engine determenistic somehow, each frame(or more frequent) store game inputs, create ReplaytInputHandler, create ReplayWriter, to write to stream(game inputs, tetromino order, timestamps delta, hold, hext, level, mode)|||
|✨ Fix unit tests for snake sequences|||
|💡 Initial hold system, Initial rotation system or charging for movements and for hold, probably goes with THAT delay on new tetromino entry|||
|💡✨ There is a short time gap between the Tetrimino placed and the next spawned. This interval is called the entry delay, or usually called ARE (a term in Japanese indicating “that” gap). This is a gap where you can charge your movement, so when the Tetrimino comes out, it will automatically shift in that direction. |||
|💡(?) Make HistoryStack concurrent|||
|💡(?) Recongnize different BTB's|||
|✨ RotateLeftOrRight(): Make default rotation as part of kicks table so this methodd can be simplified|||
|✨ Make XAML window resizable|||
|✨ Move all basic moves (<0) to separate enum to use in playfield history|||
|✨ In playfield history use Action.Landed instaed of mino property|||
|✨ Check that T spin wil be rewarded if teromino not lands and disappears(contininues moving)|||
|💡 Probably I do too many checks and code can be simplified|||
|✨ ObjectCopier: check obsolete code|||
|✨ Fix spelling of 'tetromino' everywhere, or rebrand to Block|||
|✨ Playtest variable levels up|||
|✨ 20G does not have a ghost Tetrimino function for you. When you see the Tetrimino on the ground, it’ll be too late. But if your movement gets fast enough, you’ll get more time to look at the NEXTs. Remember,|||
|||🔧 Check infinity, looks like after hard drop it waits for some time to spawn next mino and make sure that kick triggers infinity|
|||🔧 Clicking on 'New game' after another game started causes +50mb ram|
|||🔧 Level switched to 2 only after 20th mino|
|||✨ Figure out snake sequences and what to do with them in spawn queue (more than 2 snakes consequently not permited)|
|||✨ Icon for xaml project|
|||✨ Tests (maybe?)|
|||✨ Make more appropriate layout in xaml|
|||✨ Read about fourth kick upgrading simple move to diffucult move, so it won't break the combo and b2b counter|
|||✨ Use Debug.WriteLineIf everywhere and look at config for toggle|
|||🔥 Implement SRS rotation|
|||🔥 Implement gravity and speed levels|
|||🔥 Ghost tetromino|
|||🔥 Implement new cell type for playfield to be rendered, to it can have move properties|
|||🔥 Game status event|
|||🔥 Sound events|
|||🔥 Implement wallkicks|
|||🔥 Constrained(time/repetition) infinity(With toggle) https://tetris.fandom.com/wiki/Infinity|
|||🔥 Fix windows spawn location|
|||🔥 Make xaml next queue preview|
|||🔥 Xaml hold preview|
|||🔥 Xaml menu: start\restart\pause on esc|
|||🔥 SpawnTetromino(): Implement 7 pack spawn system|
|||🔥 Check performance|
|||🔥 Create readme|
|||🔥 Implement basic scoring|
|||🔥 Settings class with defaults in txt file|
|||🔥 Report actions to scoring counter|
|||🔥 Implement debug playfield states|
|||🔥 Check infinity again|
|||🔥 Check that soft drop does not lock mino|
|||🔥 Before assigning regular score I need to check if it is BTB, if so, then score BTB(actionScore*1.5) instead|
|||🔥 Auto repeat rate for rotation\horizontal and vertical movement|
|||🔥 Delayed Auto Shift: the amount of time before an input is repeated/ARR is engaged.(Auto Repeat Rate)|