using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : EnemyBehaviour
{
    public float chaseSpeed, randomSpeedRange, nonAggroSpeed, jumpGravity, fallGravity, jumpDistance, minJumpHeight, jumpOvershootHeight, attackRadius, aggroWanderDist;
    public AudioPlayer RunAudio;

    float direction{get=>transform.localScale.x; set=>transform.localScale = new Vector3(value, transform.localScale.y, transform.localScale.z);}
    float currentChaseSpeed;

    public void OnLoad(EnemySave save)
    {
        jumpTime = save.jumpTime;
        jumpVelocity = save.jumpVelocity;
    }

    public EnemySave OnSave(EnemySave save)
    {
        save.jumpTime = jumpTime;
        save.jumpVelocity = jumpVelocity;
        return save;
    }

    void Update()
    {
        Pathfind();
    }

    Path currentPath;
    List<Link> pathLinks;
    Vector2 targetPos;
    Link currentLink{get=>pathLinks[0];}

    Vector2 PointOnPath(Vector2 point, Path path)
    {
        return new Vector2(Mathf.Clamp(point.x, path.start.x + enemy.halfWidth, path.end.x - enemy.halfWidth), path.start.y);
    }

    public void StartChase()
    {
        currentChaseSpeed = Random.Range(chaseSpeed - randomSpeedRange, chaseSpeed + randomSpeedRange);
        if(jumpTime > 0f)
            return;
        if(currentPath == null)
            currentPath = PathManager.ClosestPathTo(transform.position);
        targetPath = null;
        FindNewPath();
    }

    public void StopChase()
    {
        FindNewPath();
    }

    Path targetPath;
    bool FindNewPath()
    {
        Path target = (enemy.aggro?PathManager.ClosestPathTo(enemy.Target.position):enemy.patrolPath);
        if(targetPath == target)
            return false;
        targetPath = target;
        if(target == null || target == currentPath)
        {
            pathLinks = new List<Link>();
            SetTargetPos();
            return true;
        }
        pathLinks = PathManager.PathFind(currentPath, target, transform.position, (enemy.aggro?enemy.Target.position:enemy.patrolPath.start));
        SetTargetPos();
        return true;
    }

    void SetTargetPos()
    {
        if(pathLinks.Count == 0)
        {
            Vector2 pos = PointOnPath(enemy.Target.lastSeenPosition, currentPath);
            if(targetPos == pos)
            {
                targetPos = PointOnPath(pos + Vector2.right * Random.Range(-aggroWanderDist, aggroWanderDist), currentPath);
                return;
            }
            targetPos = pos;
            return;
        }

        targetPos = JumpPosition(currentLink.selfType, currentLink.linkedType, currentLink.linkedPath, currentPath);
        currentChaseSpeed = Random.Range(chaseSpeed - randomSpeedRange, chaseSpeed + randomSpeedRange);
    }

    void Pathfind()
    {
        if(jumpTime > 0f)
        {
            Jumping();
            return;
        }

        if(!enemy.aggro && currentPath == enemy.patrolPath)
        {
            enemy.StartPatrol();
            return;
        }

        if((Vector2)transform.position == targetPos)
        {
            SetNextTarget();
            return;
        }
        if(Vector2.SqrMagnitude(enemy.Target.position - (Vector2)transform.position) < attackRadius * attackRadius)
        {
            SetTargetPos();
        }

        direction = Mathf.Sign(targetPos.x - transform.position.x);
        transform.position = Vector2.MoveTowards(transform.position, targetPos, (enemy.aggro?currentChaseSpeed:nonAggroSpeed) * Time.deltaTime);
        enemy.animator.Play(enemy.animator.runAnim);
        RunAudio.Play();
    }

    void SetNextTarget()
    {
        if(!FindNewPath())
        {
            if(pathLinks.Count > 0)
            {
                JumpTo(JumpPosition(currentLink.linkedType, currentLink.selfType, currentPath, currentLink.linkedPath));
                currentPath = currentLink.linkedPath;
                pathLinks.RemoveAt(0);
            }
            SetTargetPos();
        }
    }

    Vector2 JumpPosition(LinkType self, LinkType linked, Path current, Path path)
    {
        if(self == LinkType.left)
            return PointOnPath(path.start, path);
        if(self == LinkType.right)
            return PointOnPath(path.end, path);
        if(linked == LinkType.left)
            return PointOnPath(current.start + Vector2.left *jumpDistance, path);
        if(linked == LinkType.right)
            return PointOnPath(current.end + Vector2.right *jumpDistance, path);
        print("both links are middle");
        return Vector2.zero;
    }

    float jumpTime;
    Vector2 jumpVelocity;
    void JumpTo(Vector2 point)
    {
        float jumpApex = Mathf.Max(Mathf.Min(transform.position.y, point.y) + minJumpHeight, Mathf.Max(transform.position.y, point.y) + jumpOvershootHeight);
        float jumpHeight = jumpApex - transform.position.y;
        float fallHeight = jumpApex - point.y;
        jumpVelocity.y = Mathf.Sqrt(2f * jumpGravity * jumpHeight);
        jumpTime = (2f * jumpHeight / jumpVelocity.y) + (2f * fallHeight / Mathf.Sqrt(2f * fallGravity * fallHeight));
        jumpVelocity.x = (point.x - transform.position.x) / jumpTime;
        direction = Mathf.Sign(jumpVelocity.x);
    }

    void Jumping()
    {
        transform.position += (Vector3)jumpVelocity * Time.deltaTime;

        bool jump = jumpVelocity.y > 0f;
        jumpVelocity.y -= (jump?jumpGravity:fallGravity) * Time.deltaTime;
        enemy.animator.Play(jump?enemy.animator.jumpAnim:enemy.animator.fallAnim);

        jumpTime -= Time.deltaTime;
        if(jumpTime <= 0f)
        {
            if(currentPath == null)
            {
                currentPath = PathManager.ClosestPathTo(transform.position);
                FindNewPath();
            }
            transform.position = PointOnPath(transform.position, currentPath);
        }
    }
}
