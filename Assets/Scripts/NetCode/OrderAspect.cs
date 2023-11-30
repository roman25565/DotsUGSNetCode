using Unity.Entities;


public readonly partial struct OrderAspect : IAspect
{
    public readonly DynamicBuffer<OrderEntityBuffer> orderEntityBuffers;
    public readonly RefRO<Order> order;
}