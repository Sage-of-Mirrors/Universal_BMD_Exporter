# Universal BMD Exporter
A program designed to use Assimp (Open Asset Import Library) to convert a wide range of models into the BMD format.

BMD (short for Binary MoDel) is the model format used in many of Nintendo's games from the GameCube on, including Super Mario Sunshine, The Legend of Zelda: The Wind Waker, and Super Mario Galaxy. There is also BDL (short for Binary moDeL), which is just the BMD format with an added section for pre-compiled FIFO instructions. This exporter, for the forseeable future, only supports conversion to BMD.

Currently, the exporter supports the following attributes:

 * Textures and UVs (Up to 8 pairs per vertex)
 * Vertex colors (Up to 2 colors per vertex)
 * Normals

These attributes are currently not supported, but will likely be in the future:

 * Skeleton support

These are things that are ideas, but may not be implemented:

 * Dummy MDL3 chunk for BDL compatibility
