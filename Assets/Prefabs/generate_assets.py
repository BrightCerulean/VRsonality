import trimesh
import numpy as np
import os

out = "/mnt/documents/assets"
os.makedirs(out, exist_ok=True)

def box(dims, pos, color, rx=0, ry=0, rz=0):
    m = trimesh.creation.box(extents=dims)
    R = trimesh.transformations.euler_matrix(rx, ry, rz)
    R[:3,3] = pos
    m.apply_transform(R)
    m.visual.face_colors = [int(color[1:3],16),int(color[3:5],16),int(color[5:7],16),255]
    return m

def cyl(r, h, pos, color, rx=0, ry=0, rz=0, segs=16):
    m = trimesh.creation.cylinder(radius=r, height=h, sections=segs)
    R = trimesh.transformations.euler_matrix(rx, ry, rz)
    R[:3,3] = pos
    m.apply_transform(R)
    m.visual.face_colors = [int(color[1:3],16),int(color[3:5],16),int(color[5:7],16),255]
    return m

def sphere(r, pos, color):
    m = trimesh.creation.icosphere(subdivisions=2, radius=r)
    m.apply_translation(pos)
    m.visual.face_colors = [int(color[1:3],16),int(color[3:5],16),int(color[5:7],16),255]
    return m

def cone(r, h, pos, color, segs=8):
    m = trimesh.creation.cone(radius=r, height=h, sections=segs)
    m.apply_translation(pos)
    m.visual.face_colors = [int(color[1:3],16),int(color[3:5],16),int(color[5:7],16),255]
    return m

def save(meshes, path):
    scene = trimesh.Scene(meshes)
    scene.export(path, file_type='glb')
    print(f"✓ {path} ({os.path.getsize(path)} bytes)")

mg="#888888"; md="#555555"; red="#d94040"; blue="#4080d0"; yel="#f0c030"
orange="#e07020"; wood="#8b6f4e"; sand="#e8d5a3"

# SLIDE
ms = []
ms.append(box([0.08,3,0.08],[-0.5,1.5,-1],mg))
ms.append(box([0.08,3,0.08],[0.5,1.5,-1],mg))
for y in [0.5,1,1.5,2,2.5]: ms.append(cyl(0.03,1,[0,y,-1],md,rz=np.pi/2))
ms.append(box([1.5,0.1,1.2],[0,3,-0.5],blue))
ms.append(box([0.06,0.8,1.2],[-0.7,3.4,-0.5],yel))
ms.append(box([0.06,0.8,1.2],[0.7,3.4,-0.5],yel))
ms.append(box([1,0.05,4],[0,1.5,1.5],red,rx=0.55))
ms.append(box([0.08,0.3,4],[-0.55,1.6,1.5],red,rx=0.55))
ms.append(box([0.08,0.3,4],[0.55,1.6,1.5],red,rx=0.55))
save(ms, f"{out}/Slide.glb")

# SWINGSET
ms = []
for x,z,rx in [(-2,0.08,0.15),(-2,-0.08,-0.15),(2,0.08,0.15),(2,-0.08,-0.15)]:
    ms.append(cyl(0.06,3.8,[x,1.8,z],mg,rx=rx))
ms.append(cyl(0.06,4.5,[0,3.6,0],mg,rz=np.pi/2))
for x,c in [(-0.7,orange),(0.7,blue)]:
    ms.append(cyl(0.015,2.4,[x-0.2,2.3,0],md))
    ms.append(cyl(0.015,2.4,[x+0.2,2.3,0],md))
    ms.append(box([0.6,0.05,0.25],[x,1.1,0],c))
save(ms, f"{out}/SwingSet.glb")

# SEESAW
ms = [cone(0.3,0.6,[0,0.3,0],mg), box([4,0.1,0.4],[0,0.65,0],yel)]
for x in [-1.7,1.7]: ms.append(cyl(0.03,0.4,[x,0.85,0],md))
save(ms, f"{out}/Seesaw.glb")

# MONKEYBARS
ms = []
for x,z in [(-2,-0.5),(-2,0.5),(2,-0.5),(2,0.5)]: ms.append(cyl(0.05,2.6,[x,1.3,z],mg))
ms.append(box([4.2,0.06,0.06],[0,2.6,-0.5],mg))
ms.append(box([4.2,0.06,0.06],[0,2.6,0.5],mg))
for i in range(8): ms.append(cyl(0.03,1,[-1.75+i*0.5,2.6,0],orange,rx=np.pi/2))
save(ms, f"{out}/MonkeyBars.glb")

# TREE
save([cyl(0.2,3,[0,1.5,0],wood), sphere(1.5,[0,3.5,0],"#3d8b37")], f"{out}/Tree.glb")

# BENCH
save([box([2,0.08,0.5],[0,0.5,0],wood), box([2,0.5,0.06],[0,0.9,-0.22],wood,rx=0.15),
      box([0.08,0.5,0.5],[-0.8,0.25,0],md), box([0.08,0.5,0.5],[0.8,0.25,0],md)], f"{out}/Bench.glb")

# SANDBOX
ms = [cyl(2.5,0.04,[0,0.02,0],sand,segs=32)]
for a in [0,np.pi/2,np.pi,np.pi*1.5]:
    ms.append(box([3.4,0.3,0.15],[np.cos(a)*2.4,0.15,np.sin(a)*2.4],wood,ry=-a+np.pi/2))
save(ms, f"{out}/Sandbox.glb")

# CRIB
bw="#deb887"; bar="#c4a672"; matt="#fff8dc"
ms = [box([1.4,0.1,0.8],[0,0.4,0],bw), box([1.3,0.12,0.7],[0,0.52,0],matt)]
ms += [box([0.06,0.7,0.8],[-0.7,0.75,0],bw), box([0.06,0.7,0.8],[0.7,0.75,0],bw)]
ms += [box([1.4,0.7,0.06],[0,0.75,-0.4],bw), box([1.4,0.7,0.06],[0,0.75,0.4],bw)]
for i in range(6):
    ms.append(cyl(0.015,0.5,[-0.55+i*0.22,0.75,0.4],bar))
    ms.append(cyl(0.015,0.5,[-0.55+i*0.22,0.75,-0.4],bar))
for x,y,z in [(-0.65,0.2,-0.35),(-0.65,0.2,0.35),(0.65,0.2,-0.35),(0.65,0.2,0.35)]:
    ms.append(cyl(0.04,0.4,[x,y,z],bw))
save(ms, f"{out}/Crib.glb")

# DRESSER
ms = [box([1,0.8,0.5],[0,0.4,0],bw)]
for y in [0.15,0.45,0.65]:
    ms.append(box([0.9,0.2,0.02],[0,y,0.26],bar))
    ms.append(sphere(0.02,[0,y,0.28],"#ffd700"))
save(ms, f"{out}/Dresser.glb")

# BOOKSHELF
ms = [box([0.8,1,0.25],[0,0.5,0],bw)]
for y in [0.25,0.5,0.75]: ms.append(box([0.76,0.03,0.23],[0,y,0],bw))
for i,c in enumerate(["#d94040","#4080d0","#5a9e4b","#f0c030","#e07020"]):
    ms.append(box([0.06,0.18,0.15],[-0.25+i*0.12,0.35,0],c))
save(ms, f"{out}/Bookshelf.glb")

# MOBILE
ms = [cyl(0.01,0.8,[0,1.2,0],mg,rz=np.pi/2)]
for x,c,geo in [(-0.3,"#ff6666","sphere"),(0,"#66aaff","box"),(0.3,"#ffff66","cone")]:
    ms.append(cyl(0.005,0.2,[x,1.1,0],mg))
    if geo=="sphere": ms.append(sphere(0.06,[x,0.95,0],c))
    elif geo=="box": ms.append(box([0.1,0.1,0.1],[x,0.95,0],c))
    else: ms.append(cone(0.06,0.1,[x,0.95,0],c))
save(ms, f"{out}/Mobile.glb")

# TOYS
ms = [sphere(0.12,[0,0.12,0],"#ff4444")]
for x,c,yo in [(0.4,"#4488ff",0),(0.55,"#ffaa22",0.05),(0.45,"#44bb44",0.1)]:
    ms.append(box([0.12,0.12,0.12],[x,0.06+yo,0.3],c))
for i,(r,c) in enumerate([(0.15,"#ff6666"),(0.12,"#ffaa44"),(0.09,"#44cc44"),(0.06,"#4488ff")]):
    ms.append(cyl(r,0.05,[-0.4,0.025+i*0.055,0.3],c))
ms.append(cyl(0.015,0.25,[-0.4,0.125,0.3],wood))
save(ms, f"{out}/Toys.glb")

# PHOTOROOM - single frame on easel
ms = []
st="#333333"; fr="#222222"; cv="#eeeeee"; pl="#4488ff"
for px,pz,rx in [(-0.15,0.15,0.1),(0.15,0.15,0.1),(0,-0.1,-0.15)]:
    ms.append(cyl(0.025,2.2,[px,1.1,pz],st,rx=rx))
ms.append(box([1.5,1.9,0.05],[0,1.5,0],fr))
ms.append(box([1.3,1.7,0.01],[0,1.5,0.03],cv))
ms.append(box([0.4,0.08,0.02],[0,1.5,0.05],pl))
ms.append(box([0.08,0.4,0.02],[0,1.5,0.05],pl))
save(ms, "/mnt/documents/PhotoRoom.glb")

print("\n✅ All done!")
