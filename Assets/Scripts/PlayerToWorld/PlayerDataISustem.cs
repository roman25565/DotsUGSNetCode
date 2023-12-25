using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
[UpdateInGroup(typeof(GhostInputSystemGroup))]
public partial class PlayerDataISystem : SystemBase {
    
    private bool _isDraggingMouseBox = false;
    private bool _bothMouseButtonsPressed = false;
    private float3 _startbothMouseButtonsPressed;
    
    private Vector3 _dragStartPosition;
    
    Ray _ray;
    RaycastHit _raycastHit;

    NativeList<Entity> SelectedEntity;
    public int myId;
    private bool IsEnabled = false;
    private bool _late;
    private bool MouseButtonDown = false;
    private bool afterIsEnabled = false;
    private float3 lastraycastHit;
    private float mousePositionlate;
    private static Entity NewBuilding;
    private int _id;

    private Entity _entity;

    private DynamicBuffer<BuilderPrefabs> builderPrefabs;
    
    protected override void OnCreate()
    { 
        SelectedEntity = new NativeList<Entity>(Allocator.Persistent);
        
        RequireForUpdate<PlayerData>();
        RequireForUpdate<BuilderPrefabs>();
    }

    protected override void OnUpdate()
    {
        myId= SystemAPI.GetSingleton<PlayerData>().playerid;
        if (IsEnabled)
        {
            Physics.Raycast(
                Camera.main.ScreenPointToRay(Input.mousePosition),
                out _raycastHit,
                1000f);
            if(!MouseButtonDown)
            {
                mousePositionlate = Input.mousePosition.x;
                SystemAPI.SetComponent(NewBuilding, LocalTransform.FromPosition(_raycastHit.point));
                lastraycastHit = _raycastHit.point;
            }
            if (Input.GetMouseButton(0))
            {
                SystemAPI.SetComponent(NewBuilding, LocalTransform.FromPosition(lastraycastHit).RotateY(
                    (Input.mousePosition.x - mousePositionlate) * 0.01f));
                MouseButtonDown = true;
                _late = true;
            }
            if (Input.GetMouseButtonUp(0) && _late)
            {
                float3 newTargetPoz = SystemAPI.GetComponent<LocalTransform>(NewBuilding).Position;
                var rotation = SystemAPI.GetComponent<LocalTransform>(NewBuilding).Rotation;
                foreach (var order
                         in SystemAPI.Query<RefRW<InputOrder>>().WithAll<GhostOwnerIsLocal>())
                {
                    order.ValueRW.type = _id;
                    order.ValueRW.poz1 = newTargetPoz;
                    order.ValueRW.poz2 = new float3(rotation.value.y,rotation.value.w,0);
                }
                
                _CancelPlacedBuilding();
                EntityManager.DestroyEntity(NewBuilding);
                // SystemAPI.SetComponent(NewBuilding, new ConstructionProgress{progress = -1,myBilder = _entity});
                // SystemAPI.SetComponent(_entity, new TargetPosition{value = newTargetPoz});
                // SystemAPI.SetComponent(_entity, new ArrivalAction{value = -2});
                afterIsEnabled = true;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                EntityManager.DestroyEntity(NewBuilding);
                _CancelPlacedBuilding();
            }
        }
        builderPrefabs = SystemAPI.GetSingletonBuffer<BuilderPrefabs>();
        
        if (Input.GetMouseButtonDown(0))
        {
            _isDraggingMouseBox = true;
            _dragStartPosition = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (_isDraggingMouseBox && _dragStartPosition != Input.mousePosition && !IsEnabled)
                _SelectUnitsInDraggingBox();
            if (_isDraggingMouseBox && _dragStartPosition == Input.mousePosition)
                _SelectUnitsOfClick();
            
            _isDraggingMouseBox = false;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            DeselectAll();
        if (Input.GetMouseButton(0) && Input.GetMouseButton(1))
        {
            _bothMouseButtonsPressed = true;
            _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(
                _ray,
                out _raycastHit,
                1000f
            );
            _startbothMouseButtonsPressed = _raycastHit.point;
        }

        if (Input.GetMouseButtonUp(1) && !_bothMouseButtonsPressed)
        {
            _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(
                _ray,
                out _raycastHit,
                1000f
            );
            foreach (var order
                     in SystemAPI.Query<RefRW<InputOrder>>().WithAll<GhostOwnerIsLocal>())
            {
                order.ValueRW.type = 101;
                order.ValueRW.poz1 = _raycastHit.point;
            }
        }
    }
    private void _SelectUnitsInDraggingBox()
    {
        if (afterIsEnabled)
        {
            afterIsEnabled = false;
            return;
        }
        DeselectAll();
            
        Bounds selectionBounds = Utils.GetViewportBounds(
            Camera.main,
            _dragStartPosition,
            Input.mousePosition,
            true
        );
        foreach (var inputOrderEntityPoz in SystemAPI
                     .Query<RefRW<InputOrderEntityPoz>>().WithAll<GhostOwnerIsLocal>())
        {
            inputOrderEntityPoz.ValueRW.center = selectionBounds.center;
            inputOrderEntityPoz.ValueRW.size = selectionBounds.size;
        }

        bool inBounds;
        int priority = 0;
        int gold = SystemAPI.GetSingleton<PlayerData>().gold;
        foreach (var (localTransform ,entity) in SystemAPI.Query<RefRO<LocalTransform>>()
                     .WithEntityAccess().WithAny<MovementSpeed,ProductionProgress>().WithAll<SelectedTag>())
        {
            if (SystemAPI.HasComponent<ArrivalAction>(entity))
                if (SystemAPI.GetComponent<ArrivalAction>(entity).value == -3) 
                    continue;
            inBounds = selectionBounds.Contains(
                localTransform.ValueRO.Position
            );
            if (!inBounds)
                continue;
            if(entity == Entity.Null)
                continue;
            Debug.Log(localTransform.ValueRO.Position);
            var ifSquadEntity = SystemAPI.HasComponent<Parent>(entity) ? SystemAPI.GetComponent<Parent>(entity).Value : entity;
            var oUnitPriorityNum = SystemAPI.GetComponent<UnitPriorityComponent>(ifSquadEntity).priority;
            var unitOwnerId = SystemAPI.GetComponent<UnitOwnerComponent>(ifSquadEntity).OwnerId;
            
            if(unitOwnerId == myId) 
                SelectedEntity.Add(ifSquadEntity);
            
            if(oUnitPriorityNum > priority)
            {
                priority = oUnitPriorityNum;
                _entity = ifSquadEntity;
            }
        }
        if(_entity != Entity.Null)
            if (SystemAPI.GetComponent<UnitOwnerComponent>(_entity).OwnerId == myId) 
                _MainSelectUnitToUISkills(_entity);
    }

    private void _SelectUnitsOfClick()
    {
        if (afterIsEnabled)
        {
            afterIsEnabled = false;
            return;
        }
        var _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(
               _ray,
               out _raycastHit,
               1000f))
        {
            DeselectAll();
            _entity = Entity.Null;
            float b = 1;
            
            foreach (var inputOrderEntityPoz in SystemAPI
                         .Query<RefRW<InputOrderEntityPoz>>().WithAll<GhostOwnerIsLocal>())
            {
                inputOrderEntityPoz.ValueRW.size = _raycastHit.point;
                inputOrderEntityPoz.ValueRW.center = new Vector3();
            }
            foreach (var (unit, entity) in SystemAPI.Query<RefRO<LocalTransform>>().WithAny<MovementSpeed,ProductionProgress>()
                         .WithEntityAccess())
            {
                if(SystemAPI.HasComponent<ArrivalAction>(entity))
                    if(SystemAPI.GetComponent<ArrivalAction>(entity).value == -3)
                        continue;
                
                var ifSquadEntity = SystemAPI.HasComponent<Parent>(entity) ? SystemAPI.GetComponent<Parent>(entity).Value : entity;
                float a = math.distance(unit.ValueRO.Position, _raycastHit.point);

                if (a < b)
                {
                    b = a;
                    _entity = ifSquadEntity;
                }
            }
            if(_entity != Entity.Null)
                if (SystemAPI.GetComponent<UnitOwnerComponent>(_entity).OwnerId == myId)
                {
                    SelectedEntity.Add(_entity);
                    
                    _MainSelectUnitToUISkills(_entity);
                }
            //else
        }
    }

    private string SkillSwitch(int id)
    {
        string result = "_";
        switch (id)
        {
            case 0:
                result = "0Castle";
                break;
            case 1:
                result = "1Tower";
                break;
            case 2:
                result = "2Castle 2";
                break;
            case 3:
                result = "sada";
                break;
            case 4:
                result = "sada";
                break;
            case 5:
                result = "sada";
                break;
            case 6:
                result = "sada";
                break;
            case 20:
                result = "20CancelBuilding";
                break;
            case 21:
                result = "21DestroyBuilding";
                break;
            case 22:
                result = "22HumansSwordsmen";
                break;
            default:
                Debug.LogError(id +" Skill_404");
                break;
        }

        return result;
    }

    private void ButtonAddOnClick(int id)
    {
        _id = id;
        NewBuilding = EntityManager.Instantiate(builderPrefabs[id].Value);
        SystemAPI.SetComponent(NewBuilding, new UnitOwnerComponent{OwnerId = 1});
        EntityManager.AddComponent<ConstructionProgress>(NewBuilding);
        SystemAPI.SetComponent(NewBuilding, new ConstructionProgress{progress = -1});
        
        _late = false;
        IsEnabled = true;
        UnitsSelection.IsPreBuild = true;   
    }

    private void StartProduction(int id,Entity entity)
    {
        var dynamicBuffer = SystemAPI.GetBuffer<SkillsComponent>(entity);
        var i = 0;
        foreach (var skillsComponent in dynamicBuffer)
        {
            if (skillsComponent.skillsId == id) 
                break;
            i++;
        }
        
        foreach (var order
                 in SystemAPI.Query<RefRW<InputOrder>>().WithAll<GhostOwnerIsLocal>())
        {
            order.ValueRW.type = i + 300;
            
            order.ValueRW.poz1.x++;
        }
    }

    private void CancelButtonAddOnClick(Entity entity)
    {
        var builder = SystemAPI.GetComponent<ConstructionProgress>(entity).myBilder;
        SystemAPI.SetComponent(builder, new ArrivalAction{value = -1});
        var builderMesh = SystemAPI.GetBuffer<Child>(builder)[0].Value;
        SystemAPI.SetComponent(builderMesh, new MaterialMeshInfo{Mesh = SystemAPI.GetComponent<NormalMesh>(builder).normalMesh,Material = SystemAPI.GetComponent<MaterialMeshInfo>(builderMesh).Material});
        EntityManager.DestroyEntity(entity);
        UIManager.DestroyButtonOnSkillsPanel();
        
    }

    private void DeselectAll()
    {
        SelectedEntity.Clear(); 
        UIManager.DestroyChildGameObject();
        UIManager.DestroyButtonOnSkillsPanel();
        UIManager.DestroyButtonOnUnits();
    }
    
    private void _CancelPlacedBuilding()
    {
        
        // destroy the "phantom" building
        IsEnabled = false;
        UnitsSelection.IsPreBuild = false;
        MouseButtonDown = false;
    }

    private void _MainSelectUnitToUISkills(Entity entity)
    {
        bool isBuilding = SystemAPI.HasComponent<ConstructionProgress>(entity);
        var dynamicBuffer = SystemAPI.GetBuffer<SkillsComponent>(entity);
        var position = SystemAPI.GetComponent<LocalTransform>(entity).Position;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(position);
        if (isBuilding)
        {
            var b = UIManager.createButtonOnUnit(screenPos,Resources.Load<SkillData>($"ScriptableObject/Skills/{SkillSwitch(20)}"));
            b.onClick.AddListener(() => CancelButtonAddOnClick(entity));
            b = UIManager.createButtonOnSkillsPanel(Resources.Load<SkillData>($"ScriptableObject/Skills/{SkillSwitch(20)}"),5);
            b.onClick.AddListener(() => CancelButtonAddOnClick(entity));
            return;
        }
        var count = dynamicBuffer.Length;
        foreach (var skillsId in dynamicBuffer)
        {
            
            var skillData = Resources.Load<SkillData>($"ScriptableObject/Skills/{SkillSwitch(skillsId)}");
            
            switch (skillData.type)
            {
                case SkillType.BUILDER:
                    var b = UIManager.createTable(skillData);
                    b.onClick.AddListener(() => ButtonAddOnClick(skillsId));
                    break;
                case SkillType.UNIT_BUILD :
                    b = UIManager.createButtonOnUnits(screenPos,count,skillData);
                    b.onClick.AddListener(() => StartProduction(skillsId,entity));
                    b = UIManager.createButtonOnSkillsPanel(skillData);
                    b.onClick.AddListener(() => StartProduction(skillsId,entity));
                    break;
                case SkillType.UNIT :
                    b = UIManager.createButtonOnSkillsPanel(skillData);
                    // b.onClick.AddListener(() => ButtonAddOnClick(skillsId));not its
                    break;
            }
        }
    }
}

