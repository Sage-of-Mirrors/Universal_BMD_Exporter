# Universal BMD Exporter
A program designed to use Assimp (Open Asset Import Library) to convert a wide range of models into the BMD format.

BMD (short for Binary MoDel) is the model format used in many of Nintendo's games from the GameCube on, including Super Mario Sunshine, The Legend of Zelda: The Wind Waker, and Super Mario Galaxy. There is also BDL (short for Binary moDeL), which is just the BMD format with an added section for pre-compiled FIFO instructions. This exporter, for the forseeable future, only supports conversion to BMD.

Currently, the exporter supports the following attributes:

 * Textures and UVs (Up to 8 pairs per vertex)
 * Vertex colors (Up to 2 colors per vertex)
 * Normals

It was originally intended that Universal BMD Exporter would support skinned meshes, but this has been dropped in favor of including it in a newer BMD exporter, BMDCubed. While there is no release yet, the repo for BMDCubed can be found here: https://github.com/Sage-of-Mirrors/BMDCubed
