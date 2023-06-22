using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathManager : MonoBehaviour
{
    public static PathManager instance;

    public Tilemap PathMap, LinkMap;
    public List<Path> Paths;

    void Awake()
    {
        instance = this;

        PathMap.GetComponent<TilemapRenderer>().enabled = false;
        LinkMap.GetComponent<TilemapRenderer>().enabled = false;

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

        public PathFindLink(Link link, Path path, PathFindLink parent)
        {
            this.link = link;
            this.path = path;
            this.parent = parent;
        }
    }

    public static List<Link> PathFind(Path start, Path target, Vector2 targetPos)
    {
        List<Link> links = new List<Link>();
        if(start == target)
            return links;

        HashSet<Path> explored = new HashSet<Path>();
        List<PathFindLink> openSet = new List<PathFindLink>();

        openSet.Add(new PathFindLink(null, start, null));
        PathFindLink current, closest = null;
        float distance = Mathf.Infinity;
        while(openSet.Count > 0)
        {
            current = openSet[0];
            openSet.RemoveAt(0);
            explored.Add(current.path);

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
                    links.Add(link);
                    while(current.link != null)
                    {
                        links.Add(current.link);
                        current = current.parent;
                    }
                    links.Reverse();
                    return links;
                }

                if(explored.Contains(link.linkedPath))
                    continue;

                openSet.Add(new PathFindLink(link, link.linkedPath, current));
            }
        }

        current = closest;
        while(current.link != null)
        {
            links.Add(current.link);
            current = current.parent;
        }

        links.Reverse();
        return links;
    }

    void GeneratePaths()
    {
        Paths = new List<Path>();
        Path current = null;
        foreach(Vector3Int point in PathMap.cellBounds.allPositionsWithin)
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
    }

    void GenerateLinks()
    {
        foreach(Path path in Paths)
        {
            if(LinkMap.HasTile(path.startCoord))
            {
                Path linked = FindLinkedPath(path.startCoord, Vector3Int.zero, path);
                if(linked != null && !path.HasLinkTo(linked))
                {
                    LinkType self = LinkType.left;
                    LinkType link = (linked.endCoord.x < path.startCoord.x-1?LinkType.right:LinkType.middle);
                    path.Links.Add(new Link(linked, -1f, self, link));
                    linked.Links.Add(new Link(path, 1f, link, self));
                }
            }

            if(LinkMap.HasTile(path.endCoord))
            {
                Path linked = FindLinkedPath(path.endCoord, Vector3Int.zero, path);
                if(linked != null && !path.HasLinkTo(linked))
                {
                    LinkType self = LinkType.right;
                    LinkType link = (linked.startCoord.x > path.endCoord.x+1?LinkType.left:LinkType.middle);
                    path.Links.Add(new Link(linked, 1f, self, link));
                    linked.Links.Add(new Link(path, -1f, link, self));
                }
            }
        }
    }

    Path FindLinkedPath(Vector3Int point, Vector3Int direction, Path origin)
    {
        if(PathMap.HasTile(point))
        {
            Path path = PathAt(point);
            if(path != origin)
                return path;
            if(direction != Vector3Int.zero)
                return null;
        }
        
        Vector3Int[] directions = {Vector3Int.left, Vector3Int.right, Vector3Int.down};
        for(int i = 0; i < 3; i++)
        {
            if(directions[i] == -direction)
                continue;
            if(!LinkMap.HasTile(point + directions[i]))
                continue;
            Path path = FindLinkedPath(point + directions[i], directions[i], origin);
            if(path != null)
                return path;
        }
        return null;
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

    public bool HasLinkTo(Path path)
    {
        foreach(Link link in Links)
        {
            if(link.linkedPath == path)
                return true;
        }
        return false;
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
    public Path linkedPath;
    public float direction;
    public LinkType selfType, linkedType;

    public Link(Path path, float dir, LinkType selfType, LinkType linkedType)
    {
        linkedPath = path;
        direction = dir;
        this.selfType = selfType;
        this.linkedType = linkedType;
    }

    public override string ToString()
    {
        return "Link: " + linkedPath + ", direction: " + direction + ", type: " + selfType;
    }
}

public enum LinkType{left, middle, right}