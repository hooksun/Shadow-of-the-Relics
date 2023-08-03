using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathManager : MonoBehaviour
{
    public static PathManager instance;

    public Tilemap[] PathMaps, LinkMaps;
    public List<Path> Paths;

    void Awake()
    {
        instance = this;

        foreach(Tilemap pathMap in PathMaps)
            pathMap.GetComponent<TilemapRenderer>().enabled = false;
        foreach(Tilemap linkMap in LinkMaps)
            linkMap.GetComponent<TilemapRenderer>().enabled = false;

        GeneratePaths();
        GenerateLinks();
    }

    public static Path ClosestPathTo(Vector2 point)
    {
        Vector3Int coord = new Vector3Int((int)point.x, (int)point.y, 0);

        float distance = 0f;
        Path closest = null;
        foreach(Path path in instance.Paths)
        {
            if(coord.y < path.start.y - 0.1f)
                continue;
            float newDist = path.DistanceFrom(new Vector2(coord.x, coord.y));

            if(newDist <= 1f)
                return path;

            if(closest == null || newDist < distance)
            {
                closest = path;
                distance = newDist;
            }
        }
        
        return closest;
    }

    class PathFindLink
    {
        public Link link;
        public Path path;
        public PathFindLink parent;
        public Vector2 position;

        public PathFindLink(Link link, Path path, PathFindLink parent, Vector2 position)
        {
            this.link = link;
            this.path = path;
            this.parent = parent;
            this.position = position;
        }
    }

    public static List<Link> PathFind(Path start, Path target, Vector2 startPos, Vector2 targetPos)
    {
        List<Link> links = new List<Link>();
        if(start == target)
            return links;

        HashSet<Path> explored = new HashSet<Path>();
        List<PathFindLink> openSet = new List<PathFindLink>();

        openSet.Add(new PathFindLink(null, start, null, startPos));
        PathFindLink current, closest = null;
        float distance = Mathf.Infinity;
        while(openSet.Count > 0)
        {
            current = openSet[0];
            openSet.RemoveAt(0);
            explored.Add(current.path);

            if(current.path == target)
            {
                closest = current;
                break;
            }

            float dist = current.path.DistanceFrom(targetPos);
            if(dist < distance)
            {
                distance = dist;
                closest = current;
            }

            foreach(Link link in current.path.Links)
            {
                if(link.linkedPath == target)
                {
                    openSet.Insert(0, new PathFindLink(link, link.linkedPath, current, link.LinkedPosition()));
                    break;
                }

                if(explored.Contains(link.linkedPath))
                    continue;

                openSet.Add(new PathFindLink(link, link.linkedPath, current, link.LinkedPosition()));
            }
        }

        current = closest;
        while(current.link != null)
        {
            Link close = current.link;
            float dist = Vector2.SqrMagnitude(current.position - current.parent.position);
            foreach(Link link in current.link.selfPath.Links)
            {
                if(link.linkedPath != current.link.linkedPath)
                    continue;
                float newDist = Vector2.SqrMagnitude(link.LinkedPosition() - current.parent.position);
                if(newDist < dist)
                {
                    close = link;
                    dist = newDist;
                }
            }
            links.Add(close);
            current = current.parent;
        }

        links.Reverse();
        return links;
    }

    void GeneratePaths()
    {
        Paths = new List<Path>();
        foreach(Tilemap pathMap in PathMaps)
            GeneratePath(pathMap);
    }

    void GeneratePath(Tilemap PathMap)
    {
        Path current = null;
        Vector3Int pos = PathMap.cellBounds.position, end = pos + PathMap.cellBounds.size, point = pos;
        for(point.y = pos.y; point.y < end.y; point.y++)
        {
            for(point.x = pos.x; point.x < end.x; point.x++)
            {
                if(!PathMap.HasTile(point))
                {
                    current = null;
                    continue;
                }

                if(current == null)
                {
                    current = new Path(point, point);
                    Paths.Add(current);
                    continue;
                }

                current.endCoord = point;
            }
            current = null;
        }
    }

    void GenerateLinks()
    {
        foreach(Path path in Paths)
        {
            if(HasLink(path.startCoord))
            {
                LinkedPath = new List<Path>();
                FindLinkedPath(path.startCoord, Vector3Int.zero, path);
                foreach(Path linked in LinkedPath)
                {
                    LinkType self = LinkType.left;
                    LinkType link = (linked.endCoord.x < path.startCoord.x-1?LinkType.right:LinkType.middle);
                    Link dupe = path.HasLinkTo(linked);
                    if(dupe != null && self == dupe.selfType && link == dupe.linkedType)
                        continue;
                    path.Links.Add(new Link(path, linked, -1f, self, link));
                    linked.Links.Add(new Link(linked, path, 1f, link, self));
                }
            }

            if(HasLink(path.endCoord))
            {
                LinkedPath = new List<Path>();
                FindLinkedPath(path.endCoord, Vector3Int.zero, path);
                foreach(Path linked in LinkedPath)
                {
                    LinkType self = LinkType.right;
                    LinkType link = (linked.startCoord.x > path.endCoord.x+1?LinkType.left:LinkType.middle);
                    Link dupe = path.HasLinkTo(linked);
                    if(dupe != null && self == dupe.selfType && link == dupe.linkedType)
                        continue;
                    path.Links.Add(new Link(path, linked, 1f, self, link));
                    linked.Links.Add(new Link(linked, path, -1f, link, self));
                }
            }
        }
    }

    bool HasLink(Vector3Int coord)
    {
        foreach(Tilemap linkMap in LinkMaps)
        {
            if(linkMap.HasTile(coord))
                return true;
        }
        return false;
    }

    bool HasPath(Vector3Int coord)
    {
        foreach(Tilemap pathMap in PathMaps)
        {
            if(pathMap.HasTile(coord))
                return true;
        }
        return false;
    }

    List<Path> LinkedPath;
    void FindLinkedPath(Vector3Int point, Vector3Int direction, Path origin)
    {
        if(HasPath(point))
        {
            Path path = PathAt(point);
            if(path != origin)
            {
                LinkedPath.Add(path);
                return;
            }
            if(direction != Vector3Int.zero)
                return;
        }
        
        Vector3Int[] directions = {Vector3Int.left, Vector3Int.right, Vector3Int.down};
        for(int i = 0; i < 3; i++)
        {
            if(directions[i] == -direction)
                continue;
            if(!HasLink(point + directions[i]))
                continue;
            FindLinkedPath(point + directions[i], directions[i], origin);
        }
    }

    Path PathAt(Vector3Int point)
    {
        foreach(Path path in Paths)
        {
            if(path.startCoord.y == point.y && path.startCoord.x <= point.x && path.endCoord.x >= point.x)
                return path;
        }
        return null;
    }
}

public class Path
{
    public Vector3Int startCoord, endCoord;
    public List<Link> Links;

    public Vector2 start{get=>new Vector2(startCoord.x, startCoord.y + 1);}
    public Vector2 end{get=>new Vector2(endCoord.x + 1, endCoord.y + 1);}
    
    public Path(Vector3Int startCoord, Vector3Int endCoord)
    {
        this.startCoord = startCoord;
        this.endCoord = endCoord;
        Links = new List<Link>();
    }

    public Link HasLinkTo(Path path)
    {
        foreach(Link link in Links)
        {
            if(link.linkedPath == path)
                return link;
        }
        return null;
    }

    public Vector2 PointOnPath(Vector2 point, float width = 0f)
    {
        return new Vector2(Mathf.Clamp(point.x, start.x + width, end.x - width), start.y);
    }

    public float DistanceFrom(Vector2 coord)
    {
        float newDist = 0f;
        newDist += Mathf.Max(0f, start.x - coord.x);
        newDist += Mathf.Max(0f, coord.x - end.x);
        newDist += Mathf.Abs(coord.y - start.y);
        return newDist;
    }

    public override string ToString()
    {
        return "start: " + startCoord + ", end: " + endCoord;
    }
}

public class Link
{
    public Path selfPath, linkedPath;
    public float direction;
    public LinkType selfType, linkedType;

    public Link(Path path, Path link, float dir, LinkType selfType, LinkType linkedType)
    {
        selfPath = path;
        linkedPath = link;
        direction = dir;
        this.selfType = selfType;
        this.linkedType = linkedType;
    }

    public Vector2 LinkedPosition()
    {
        if(linkedType == LinkType.left)
            return linkedPath.start;
        if(linkedType == LinkType.right)
            return linkedPath.end;
        if(selfType == LinkType.left)
            return linkedPath.PointOnPath(selfPath.start);
        return linkedPath.PointOnPath(selfPath.end);
    }

    public override string ToString()
    {
        return "Link: " + linkedPath + ", direction: " + direction + ", type: " + selfType;
    }
}

public enum LinkType{left, middle, right}