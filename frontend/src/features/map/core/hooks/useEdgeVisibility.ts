import { useEffect, useState } from "react";

const EDGE_THRESHOLD = 80;

export function useEdgeVisibility(edge: "top" | "bottom", forceVisible: boolean): boolean {
  const [hovering, setHovering] = useState(false);

  useEffect(() => {
    function handleMouseMove(e: MouseEvent) {
      const nearEdge =
        edge === "top"
          ? e.clientY <= EDGE_THRESHOLD
          : e.clientY >= window.innerHeight - EDGE_THRESHOLD;
      setHovering(nearEdge);
    }
    window.addEventListener("mousemove", handleMouseMove);
    return () => window.removeEventListener("mousemove", handleMouseMove);
  }, [edge]);

  return forceVisible || hovering;
}