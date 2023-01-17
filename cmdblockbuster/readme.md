# CmdBlockBuster  
Tetris that supports most popular [Super Rotation System](https://tetris.fandom.com/wiki/SRS)  

Features:  
 - Hard drops
 - SRS rotaion
 - Naive gravity
 - Wall kicks

 # Field
 It have 2 invisible top rows to spawn tetrominoes, it is enough SRS system

 # TODO
 - [x] Implement SRS rotation
 - [x] Implement gravity and speed levels
 - [x] Ghost tetromino
 - [x] Implement new cell type for playfield to be rendered, to it can have move properties
 - [x] Game status event
 - [x] Sound events
 - [x] Implement wallkicks
 - [x] Constrained(time/repetition) infinity(With toggle) https://tetris.fandom.com/wiki/Infinity
 - [ ] Xaml menu and restart game
 - [ ] Make xaml next queue preview
 - [ ] Xaml hold preview
 - [ ] Fix spelling of 'tetromino' everywhere, or rebrand to Block
 - [ ] Implement basic scoring
 - [ ] SpawnTetromino(): Implement 7 pack spawn system
 - [ ] Playtest variable levels up
 - [ ] Tests (maybe?)
 - [ ] Create readme
 - [ ] Check infinity, looks like after hard drop it waits for some time to spawn next mino and check that kick triggers infinity