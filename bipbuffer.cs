using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
/* 
*
* basically, we use two circular buffers, each one corresponding to a given
* direction.
*
* each buffer is implemented as two regions:
*
*   region A which is (a_start,a_end)
*   region B which is (0, b_end)  with b_end <= a_start
*
* an empty buffer has:  a_start = a_end = b_end = 0
*
* a_start is the pointer where we start reading data
* a_end is the pointer where we start writing data, unless it is BUFFER_SIZE,
* then you start writing at b_end
*
* the buffer is full when  b_end == a_start && a_end == BUFFER_SIZE
*
* there is room when b_end < a_start || a_end < BUFER_SIZE
*
* when reading, a_start is incremented, it a_start meets a_end, then
* we do:  a_start = 0, a_end = b_end, b_end = 0, and keep going on..
*/
namespace BipBufferS
{

    public class BipBuffer
    {
      
	public uint  size;
	/* region A */
	public uint  a_start;
        public uint a_end;
	/* region B */
	public uint b_end;
	/* is B inuse? */
	bool b_inuse;
	public byte [] data;

 

public uint bipbuf_get_unused_size()
{
	

	if (b_inuse)
	{
		/* distance between region B and region A */
		return a_start - b_end;
	}
	else
	{
		/* find out if we should turn on region B?
		* ie. is the distance between B and A more than A to buffer's end? */
		if (a_start - b_end > size - a_end)
		{
			b_inuse = true;
			return a_start - b_end;
		}
		else
		{
			return size - a_end;
		}
	}
}

/**
* creat new bip buffer.
* @param order to the power of two equals size*/
unsafe public bool bipbuf_new(uint len)
{
    try
    {
        data = new byte[len];


        a_start = a_end = b_end = 0;
        size = len;

        b_inuse = false;
    }
    catch(Exception ex)
    {
        return false;
    }
    return true;
}

public unsafe void Clear()
{
	
	//free(data);
	//free(me);
}

public bool bipbuf_is_empty()
{
	
	return a_start == a_end;
}
public unsafe void memcpy(byte* pTarget, byte[] source,uint count)
{
    if ((source == null) || (pTarget == null))
    {
        throw new System.ArgumentException();
    }
   
    
    if (count < 0)
    {
        throw new System.ArgumentException();
    }

    // The following fixed statement pins the location of   
    // target objects in memory so that they will not be moved by garbage 
    // collection. The source is done at initialization level
    fixed (byte* pSource = source)
    {
        // Set the starting points in source and target for the copying. 
        //we can not PSource++ on fixed pointer we need notfixed ps,pt
        byte* ps = pSource;
        byte* pt = pTarget ;

        // Copy the specified number of bytes from source to target. 
        for (int i = 0; i < count; i++)
        {
            *pt = *ps;
            pt++;
            ps++;
        }
    }
}
public unsafe void memcpy(byte* pTarget, byte* pSource,uint count)
{
    if ((pSource == null) || (pTarget == null))
    {
        throw new System.ArgumentException();
    }
   
    
    if (count < 0)
    {
        throw new System.ArgumentException();
    }

    // The following fixed statement pins the location of   
    // target objects in memory so that they will not be moved by garbage 
    // collection. The source is done at initialization level
    //fixed (byte* pSource = source)
    {
        // Set the starting points in source and target for the copying. 
        //we can not PSource++ on fixed pointer we need notfixed ps,pt
        byte* ps = pSource;
        byte* pt = pTarget ;

        // Copy the specified number of bytes from source to target. 
        for (int i = 0; i < count; i++)
        {
            *pt = *ps;
            pt++;
            ps++;
        }
    }
}


        public unsafe void memcpy(uint targetOffset, byte[] source)
{
    if ((source == null) || (data == null))
    {
        throw new System.ArgumentException();
    }
    int count = source.Length;
    int len = source.Count();
    if (targetOffset < 0 || count < 0)
    {
        throw new System.ArgumentException();
    }

    if (data.Length - targetOffset < count)
    {
        throw new System.ArgumentException();
    }

    // The following fixed statement pins the location of   
    // target objects in memory so that they will not be moved by garbage 
    // collection. The source is done at initialization level
    fixed (byte* pTarget = data, pSource = source)
    {
        // Set the starting points in source and target for the copying. 
        //we can not PSource++ on fixed pointer we need notfixed ps,pt
        byte* ps = pSource;
        byte* pt = pTarget + targetOffset;

        // Copy the specified number of bytes from source to target. 
        for (int i = 0; i < count; i++)
        {
            *pt = *ps;
            pt++;
            ps++;
        }
    }
}
        public unsafe void memcpy( byte[] destination,uint targetOffset,uint count)
        {
            if ((destination == null) || (data == null))
            {
                throw new System.ArgumentException();
            }
           // int count = destination.Length;
          

            // The following fixed statement pins the location of   
            // target objects in memory so that they will not be moved by garbage 
            // collection. The source is done at initialization level
            fixed (byte* pTarget = destination, pSource = data)
            {
                // Set the starting points in source and target for the copying. 
                //we can not PSource++ on fixed pointer we need notfixed ps,pt
                byte* ps = pSource+ targetOffset;
                byte* pt = pTarget ;

                // Copy the specified number of bytes from source to target. 
                for (int i = 0; i < count; i++)
                {
                    *pt = *ps;
                    pt++;
                    ps++;
                }
            }
        }

        static unsafe void memcpy(byte[] target, uint targetOffset, byte* source, uint count)
{
    if ((source == null) || (target == null))
    {
        throw new System.ArgumentException();
    }

    if (targetOffset < 0 || count < 0)
    {
        throw new System.ArgumentException();
    }
  
    if (target.Length - targetOffset < count)
    {
        throw new System.ArgumentException();
    }

    // The following fixed statement pins the location of   
    // target objects in memory so that they will not be moved by garbage 
    // collection. The source is done at initialization level
    fixed (byte* pTarget = target)
    {
        // Set the starting points in source and target for the copying. 
        byte* ps = source;
        byte* pt = pTarget + targetOffset;

        // Copy the specified number of bytes from source to target. 
        for (int i = 0; i < count; i++)
        {
            *pt = *ps;
            pt++;
            ps++;
        }
    }
}
        public string strdebug = "";
/**
* @return number of bytes offered
**/
        public unsafe bool push(byte[] input,Action< byte[] , uint , string > action, string strdebug = null)
{
    uint len =(uint) input.Length;

	/* not enough space */
	if (len > bipbuf_get_unused_size())
		return false;

	if (b_inuse)
	{
        fixed (byte* pTarget=input)
        {
            
	action(input,b_end,strdebug);
        }
        //	memcpy(b_end,input);
    
        b_end += len;
	}
	else
	{
         //fixed (byte* pTarget=input)
        {
             
	action(input,a_end,strdebug);
        }
		//memcpy(a_end, input);
		a_end += len;
	}

	return true;
}
        public unsafe bool push_block(byte[] input)
        {
            uint len = (uint)input.Length;

            /* not enough space */
            if (len > bipbuf_get_unused_size())
                return false;

            if (b_inuse)
            {
                memcpy(b_end, input);
                b_end += len;
            }
            else
            {
                memcpy(a_end, input);
                a_end += len;
            }

            return true;
        }
        unsafe public bool Producer(byte[] input)
        {
            if(input==null)
                return false;

            uint len =(uint)input.Length;
            byte*p=null;
            uint offer = bipbuf_offer(p);
            if (len > offer)
                return false;
            if (b_inuse)
            {
                memcpy(p,input, len);
                b_end += len;
            }
            else
            {
                memcpy(p, input, len);
                a_end += size;
            }

            return true;
        }
       // Here with memory_order_seq_cst for every operation. This is overkill but easy to reason about
	//
	// Push is only changed by producer and can be safely loaded using memory_order_relexed
	//         head is updated by consumer and must be loaded using at least memory_order_acquire
	
	/*public unsafe uint push(byte* items, uint count, ConsumeTest action)
	{
		T* target = nullptr;
		bool b_inuse = switch_b();
		uint avail = avail_space(target,b_inuse);
		if (count > avail)
			count = avail;
		if (count == 0)
			return count;
			action(target, items, count);
			if (b_end > 0)
			{
				b_end += count;
			}
			else
			{
				a_end += count;
			}

		return count;
		

	}*/

	//Before we push we should check buffer status
	public bool switch_b()
	{
	/* we could to be empty.. */
	if (a_start == a_end)
	{
		/*In every case we reset a_start*/
		a_start = 0;
		
		/* replace a with region b */
		if (b_end>0)
		{
			
			a_end=b_end;
			b_end = 0;
			return false;
			
			
		}
		else
		{
			/* safely move the pointer back to the start because we are empty */
			a_start = a_end = 0;
			return false;
		}
	}
	else
	{
		/*Should we switch on region b? if is off
		
		Once more free space is available to the left of region A than to the right of it, a second region (comically named "region B") is created in that space.

		*/
		if (b_end == 0)
		{
			uint left = a_start;
			uint right = size - a_end;
			if (left>right)
				return true;
			else return false;
		}
	}
        return false;
    }
        public unsafe uint bipbuf_offer(byte* input)
        {
            fixed (byte* pTarget = data)
            {

                if (b_inuse)
                {
                    /* distance between region B and region A */
                    //we 
                    uint unused = a_start - b_end;
                    input = pTarget + a_start;
                   
                    return unused;

                }
                else
                {
                    /* find out if we should turn on region B?
                    * ie. is the distance between B and A more than A to buffer's end? */
                    if (a_start - b_end > size - a_end)
                    {
                        b_inuse = true;
                        uint unused = a_start - b_end;
                        input = pTarget + a_start;
                        return unused;

                    }
                    else
                    {
                        uint unused = size - a_end;
                        input = pTarget + a_end;
                        return unused;
                        
                    }
                }
                return 0;//when no space
            }
        }
/**
* Look at data.
* Don't move cursor
*/
        public unsafe bool bipbuf_peek(uint count, Action<uint , uint ,string > action,string strdebug=null)
{

    uint offset = 0;
   // fixed (byte* pTarget = data)
    {
        action(offset,count,strdebug);
        return true;// pTarget + a_start;
    }
}
        public  delegate void PollTest(uint offset, uint count,string strdebug=null);
        public  delegate void PushTest(byte[] p, uint offset, string strdebug = null);
        public uint Readindex = 0;
        public uint Writeindex = 0;
        public  void VisualizePush(byte[] p, uint offset, string strdebug)
        {
            Writeindex = offset;
                                  
            memcpy(offset, p);
                    
              
        }
        public  void VisualizePoll(uint offset,uint count,string strdebug=null)
       {
           Readindex = offset;
           if (strdebug == null || strdebug.Length == 0)
               return;
           byte[] output = System.Text.Encoding.UTF8.GetBytes(strdebug);
                  // fixed(byte* p=input)
                   {
                      

                       memcpy(output,offset,count);
                   }
           }
/**
* Get pointer to data to read. Move the cursor on.
*
* @return pointer to data, null if we can't poll this much data
*/
        public unsafe uint poll(uint count, Action<uint, uint, string> action)
{

  	if (a_start != a_end) //we have something in a region
			{
	         fixed (byte* pTarget = data )
             {
    
       
				uint len = a_end - a_start;
				if (count > len)
				{
                  
					action(a_start, len,strdebug);

					a_start = a_start + len;
					//change status of buffer

					return len;
				}
				else
				{
                    
					action(a_start, count,strdebug);

					a_start = a_start + count;
					//check if we need to change statusqou of buffer
					return count;
				}
            }
            }
			else
			{
				//If A is empty,check if we have something in B region
				if (b_end == 0)
				{
					return 0;
				}
             fixed (byte* pTarget = data )
             {
				if (count > b_end)
				{
					action( 0,b_end,strdebug);
					count = b_end;
					b_end = 0;//we empty both regions
					return count;
				}
				else
				{
					action(0, count,strdebug);
					b_end = b_end - count;
					return count;
				}
             }
             }


		

}
        

        public uint bipbuf_get_size()
{


	return size;
}

/**
* @return tell us how much space we have assigned */
        public uint bipbuf_get_spaceused()
{
    

	return (a_end - a_start) + (b_end);
}

    }
}
