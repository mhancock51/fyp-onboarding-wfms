import { useState } from 'react'
import { toast } from 'sonner';
import { useTheme } from 'next-themes';
import { Moon, Sun } from 'lucide-react';

export default function ThemeToggle() {  
  const { theme, setTheme} = useTheme();  

  const [hovered, setHovered] = useState<boolean>(false);

  function toggleTheme() {    
    setTheme(theme === "light" ? "dark" : "light");  
    toast(`Set theme to ${theme} mode`);
  }

  return (
    <div className='flex flex-row gap-2 items-center w-full' 
      onMouseEnter={() => {setHovered(true)}} onMouseLeave={() => {setHovered(false)}}
    >
      {
        theme === "light" && !hovered &&
        <Sun/>
      }
      {
        theme === "light" && hovered &&
        <Moon/>
      }
      {
        theme == "dark" && !hovered &&
        <Moon/>
      }
      {
        theme == "dark" && hovered &&
        <Sun/> 
      }
      <a className='rounded-xl' onClick={toggleTheme}>Toggle Theme</a>
    </div>
  )
}
