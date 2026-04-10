import trimesh
import numpy as np
import os

output_dir = os.path.dirname(__file__)

def make_box(name, size, position, hex_color, rx=0, ry=0, rz=0):
    mesh = trimesh.creation.box(extents=size)
    rotation = trimesh.transformations.euler_matrix(rx, ry, rz)
    rotation[:3, 3] = position
    mesh.apply_transform(rotation)
    r = int(hex_color[1:3], 16)
    g = int(hex_color[3:5], 16)
    b = int(hex_color[5:7], 16)
    mesh.visual.face_colors = [r, g, b, 255]
    return name, mesh

def make_cylinder(name, radius, height, position, hex_color, rx=0, ry=0, rz=0, sections=16):
    mesh = trimesh.creation.cylinder(radius=radius, height=height, sections=sections)
    rotation = trimesh.transformations.euler_matrix(rx, ry, rz)
    rotation[:3, 3] = position
    mesh.apply_transform(rotation)
    r = int(hex_color[1:3], 16)
    g = int(hex_color[3:5], 16)
    b = int(hex_color[5:7], 16)
    mesh.visual.face_colors = [r, g, b, 255]
    return name, mesh

def make_sphere(name, radius, position, hex_color):
    mesh = trimesh.creation.icosphere(subdivisions=2, radius=radius)
    mesh.apply_translation(position)
    r = int(hex_color[1:3], 16)
    g = int(hex_color[3:5], 16)
    b = int(hex_color[5:7], 16)
    mesh.visual.face_colors = [r, g, b, 255]
    return name, mesh

def make_cone(name, radius, height, position, hex_color, sections=8):
    mesh = trimesh.creation.cone(radius=radius, height=height, sections=sections)
    mesh.apply_translation(position)
    r = int(hex_color[1:3], 16)
    g = int(hex_color[3:5], 16)
    b = int(hex_color[5:7], 16)
    mesh.visual.face_colors = [r, g, b, 255]
    return name, mesh

def export_glb(named_meshes, asset_name):
    scene = trimesh.Scene()
    for name, mesh in named_meshes:
        scene.add_geometry(mesh, node_name=name)
    filepath = os.path.join(output_dir, f"{asset_name}.glb")
    scene.export(filepath, file_type='glb')
    print(f"saved: {filepath} ({os.path.getsize(filepath)} bytes)")

# colors
G   = "#888888"
DG  = "#555555"
R   = "#d94040"
B   = "#4080d0"
Y   = "#f0c030"
O   = "#e07020"
W   = "#8b6f4e"
S   = "#e8d5a3"
BE  = "#deb887"
LB  = "#c4a672"
CR  = "#fff8dc"
ST  = "#333333"
FR  = "#222222"
CV  = "#eeeeee"
PT  = "#4488ff"
TL  = "#3d8b37"
GD  = "#ffd700"
BG  = "#5a9e4b"
MS  = "#ff6666"
MB  = "#66aaff"
MC  = "#ffff66"
TB  = "#ff4444"
TO  = "#ffaa22"
TG  = "#44bb44"
RO  = "#ffaa44"
RG  = "#44cc44"

# ---- SLIDE ----
slide = []
slide.append(make_box("1",  [0.08, 3, 0.08],   [-0.5,  1.5,  -1],  G))
slide.append(make_box("2",  [0.08, 3, 0.08],   [ 0.5,  1.5,  -1],  G))
for i, y in enumerate([0.5, 1.0, 1.5, 2.0, 2.5]):
    slide.append(make_cylinder(str(3 + i), 0.03, 1, [0, y, -1], DG, rz=np.pi/2))
slide.append(make_box("8",  [1.5, 0.1,  1.2],  [ 0,    3.0,  -0.5], B))
slide.append(make_box("9",  [0.06, 0.8, 1.2],  [-0.7,  3.4,  -0.5], Y))
slide.append(make_box("10", [0.06, 0.8, 1.2],  [ 0.7,  3.4,  -0.5], Y))
slide.append(make_box("11", [1,    0.05, 4],    [ 0,    1.5,   1.5], R, rx=0.55))
slide.append(make_box("12", [0.08, 0.3,  4],   [-0.55, 1.6,   1.5], R, rx=0.55))
slide.append(make_box("13", [0.08, 0.3,  4],   [ 0.55, 1.6,   1.5], R, rx=0.55))
export_glb(slide, "Slide")

# ---- SWINGSET ----
swing = []
leg_configs = [(-2, -0.4, -np.pi/2 + 0.11), (-2, 0.4, -np.pi/2 - 0.11),
               ( 2, -0.4, -np.pi/2 + 0.11), ( 2, 0.4, -np.pi/2 - 0.11)]
for i, (x, z, rx) in enumerate(leg_configs):
    swing.append(make_cylinder(str(1 + i), 0.06, 3.8, [x, 1.8, z], G, rx=rx))
swing.append(make_cylinder("5", 0.06, 4.5, [0, 3.6, 0], G, ry=np.pi/2))
n = 6
for x, color in [(-0.7, O), (0.7, B)]:
    swing.append(make_cylinder(str(n),     0.015, 2.4, [x - 0.2, 2.3, 0], DG, rx=-np.pi/2))
    swing.append(make_cylinder(str(n + 1), 0.015, 2.4, [x + 0.2, 2.3, 0], DG, rx=-np.pi/2))
    swing.append(make_box(str(n + 2), [0.6, 0.05, 0.25], [x, 1.1, 0], color))
    n += 3
export_glb(swing, "SwingSet")

# ---- SEESAW ----
seesaw = []
seesaw.append(make_cylinder("1", 0.08, 0.5,  [ 0,    0.25, 0], G,  rx=-np.pi/2))
seesaw.append(make_box("2",      [4, 0.1, 0.4], [0,   0.65, 0], Y))
seesaw.append(make_cylinder("3", 0.03, 0.4, [-1.7,   0.85, 0], DG, rx=-np.pi/2))
seesaw.append(make_cylinder("4", 0.03, 0.4, [ 1.7,   0.85, 0], DG, rx=-np.pi/2))
export_glb(seesaw, "Seesaw")

# ---- MONKEY BARS ----
monkeybars = []
for i, (x, z) in enumerate([(-2, -0.6), (-2, 0.6), (2, -0.6), (2, 0.6)]):
    monkeybars.append(make_cylinder(str(1 + i), 0.07, 2.8, [x, 1.4, z], G, rx=-np.pi/2))
monkeybars.append(make_box("5", [4.2, 0.08, 0.08], [0, 2.8, -0.6], G))
monkeybars.append(make_box("6", [4.2, 0.08, 0.08], [0, 2.8,  0.6], G))
for i in range(7):
    monkeybars.append(make_cylinder(str(7 + i), 0.04, 1.2, [-1.5 + i * 0.5, 2.8, 0], O))
export_glb(monkeybars, "MonkeyBars")

# ---- TREE ----
tree = []
tree.append(make_cylinder("1", 0.2, 3,   [0, 1.5, 0], W,  rx=-np.pi/2))
tree.append(make_sphere("2",   1.5,       [0, 3.5, 0], TL))
export_glb(tree, "Tree")

# ---- BENCH ----
bench = []
bench.append(make_box("1", [2,    0.08, 0.5],  [ 0,    0.5,   0],    W))
bench.append(make_box("2", [2,    0.5,  0.06], [ 0,    0.9,  -0.22], W,  rx=0.15))
bench.append(make_box("3", [0.08, 0.5,  0.5],  [-0.8,  0.25,  0],    DG))
bench.append(make_box("4", [0.08, 0.5,  0.5],  [ 0.8,  0.25,  0],    DG))
export_glb(bench, "Bench")

# ---- SANDBOX ----
sandbox = []
sandbox.append(make_cylinder("1", 2.5, 0.04, [0, 0.02, 0], S, sections=32))
for i, angle in enumerate([0, np.pi/2, np.pi, np.pi * 1.5]):
    x = np.cos(angle) * 2.4
    z = np.sin(angle) * 2.4
    sandbox.append(make_box(str(2 + i), [3.4, 0.3, 0.15], [x, 0.15, z], W, ry=-angle + np.pi/2))
export_glb(sandbox, "Sandbox")

# ---- CRIB ----
crib = []
crib.append(make_box("1", [1.4, 0.1,  0.8],  [ 0,    0.4,   0],    BE))
crib.append(make_box("2", [1.3, 0.12, 0.7],  [ 0,    0.52,  0],    CR))
crib.append(make_box("3", [0.06, 0.7, 0.8],  [-0.7,  0.75,  0],    BE))
crib.append(make_box("4", [0.06, 0.7, 0.8],  [ 0.7,  0.75,  0],    BE))
crib.append(make_box("5", [1.4,  0.7, 0.06], [ 0,    0.75, -0.4],  BE))
crib.append(make_box("6", [1.4,  0.7, 0.06], [ 0,    0.75,  0.4],  BE))
n = 7
for i in range(6):
    x_pos = -0.55 + i * 0.22
    crib.append(make_cylinder(str(n),     0.015, 0.5, [x_pos, 0.75,  0.4], LB, rx=-np.pi/2))
    crib.append(make_cylinder(str(n + 1), 0.015, 0.5, [x_pos, 0.75, -0.4], LB, rx=-np.pi/2))
    n += 2
for i, (x, y, z) in enumerate([(-0.65, 0.2, -0.35), (-0.65, 0.2, 0.35),
                                 ( 0.65, 0.2, -0.35), ( 0.65, 0.2, 0.35)]):
    crib.append(make_cylinder(str(n + i), 0.04, 0.4, [x, y, z], BE, rx=-np.pi/2))
export_glb(crib, "Crib")

# ---- DRESSER ----
dresser = []
dresser.append(make_box("1", [1, 0.8, 0.5], [0, 0.4, 0], BE))
n = 2
for y in [0.15, 0.45, 0.65]:
    dresser.append(make_box(str(n),     [0.9, 0.2, 0.02], [0, y, 0.26], LB))
    dresser.append(make_sphere(str(n+1), 0.02,             [0, y, 0.28], GD))
    n += 2
export_glb(dresser, "Dresser")

# ---- BOOKSHELF ----
bookshelf = []
bookshelf.append(make_box("1", [0.8, 1, 0.25], [0, 0.5, 0], BE))
for i, y in enumerate([0.25, 0.5, 0.75]):
    bookshelf.append(make_box(str(2 + i), [0.76, 0.03, 0.23], [0, y, 0], BE))
for i, color in enumerate([R, B, BG, Y, O]):
    bookshelf.append(make_box(str(5 + i), [0.06, 0.18, 0.15], [-0.25 + i * 0.12, 0.35, 0], color))
export_glb(bookshelf, "Bookshelf")

# ---- MOBILE ----
mobile = []
mobile.append(make_cylinder("1", 0.01, 0.8, [0, 1.2, 0], G, rz=np.pi/2))
hanging = [(-0.3, MS, "sphere"), (0, MB, "box"), (0.3, MC, "cone")]
n = 2
for x, color, shape in hanging:
    mobile.append(make_cylinder(str(n), 0.005, 0.2, [x, 1.1, 0], G, rx=-np.pi/2))
    if shape == "sphere":
        mobile.append(make_sphere(str(n+1), 0.06, [x, 0.95, 0], color))
    elif shape == "box":
        mobile.append(make_box(str(n+1), [0.1, 0.1, 0.1], [x, 0.95, 0], color))
    elif shape == "cone":
        mobile.append(make_cone(str(n+1), 0.06, 0.1, [x, 0.95, 0], color))
    n += 2
export_glb(mobile, "Mobile")

# ---- TOYS ----
toys = []
toys.append(make_sphere("1", 0.12, [0, 0.12, 0], TB))
for i, (x, color, yo) in enumerate([(0.4, B, 0), (0.55, TO, 0.05), (0.45, TG, 0.1)]):
    toys.append(make_box(str(2 + i), [0.12, 0.12, 0.12], [x, 0.06 + yo, 0.3], color))
for i, (ring_r, ring_color) in enumerate([(0.15, MS), (0.12, RO), (0.09, RG), (0.06, B)]):
    toys.append(make_cylinder(str(5 + i), ring_r, 0.05, [-0.4, 0.025 + i * 0.055, 0.3], ring_color))
toys.append(make_cylinder("9", 0.015, 0.25, [-0.4, 0.125, 0.3], W, rx=-np.pi/2))
export_glb(toys, "Toys")

# ---- PHOTOROOM ----
photoroom = []
photoroom.append(make_box("1", [1.3, 1.7, 0.05], [0, 2.0,  0],    FR))
photoroom.append(make_box("2", [1.1, 1.5, 0.01], [0, 2.0,  0.04], CV))
photoroom.append(make_box("3", [0.5, 0.06, 0.02],[0, 2.0,  0.06], PT))
photoroom.append(make_box("4", [0.06, 0.5, 0.02],[0, 2.0,  0.06], PT))
photoroom.append(make_cylinder("5", 0.025, 2.2, [-0.35, 1.0,  0.25], ST, rx=-np.pi/2 + 0.22))
photoroom.append(make_cylinder("6", 0.025, 2.2, [ 0.35, 1.0,  0.25], ST, rx=-np.pi/2 + 0.22))
photoroom.append(make_cylinder("7", 0.025, 2.0, [ 0,    1.0, -0.35], ST, rx=-np.pi/2 - 0.22))
export_glb(photoroom, "PhotoRoom")

print("\ndone!")