const pods = [
    {name: "RMIT Pod", lat: -37.8093483, long: 144.9640153}
  ];
  const cars = [
    {name: "Tim the Tesla", lat: -37.8167399, long: 144.9612497},
    {name: "Tony the Tesla", lat: -37.8073874, long: 144.974175},
    {name: "Marco the Mazda", lat: -37.8195234, long: 144.9668658},
    {name: "Terry the Toyota", lat: -37.8066475, long: 144.9647881},
    {name: "Bob the BYD", lat: -37.7981268, long: 144.9513347}
  ];
  
  // Google Map's sample code as my code blew up recently - Jack
  function initAutocomplete() {
      const map = new google.maps.Map(document.getElementById("map"), {
        center: { lat: -37.8083331, lng: 144.9639386 },
        zoom: 13,
        mapTypeId: "roadmap",
      });
      // Create the search box and link it to the UI element.
      const input = document.getElementById("pac-input");
      const searchBox = new google.maps.places.SearchBox(input);
    
      // Bias the SearchBox results towards current map's viewport.
      map.addListener("bounds_changed", () => {
        searchBox.setBounds(map.getBounds());
      });
    
      // Add all pods and cars as markers
      for (let car of cars) {
        let marker = new google.maps.Marker({
          map,
          title: car.name,
          position: { lat: car.lat, lng: car.long },
        })
        const infowindow = new google.maps.InfoWindow({
          content: `<strong>${car.name}</strong><br><a href="/Bookings/Make">Book</a>`,
        });
    
        marker.addListener("click", () => {
          infowindow.open(map, marker);
        });
      }
    
      // Respond to user searches and move the map there
      searchBox.addListener("places_changed", () => {
        const places = searchBox.getPlaces();
    
        if (places.length == 0) {
          return;
        }
  
        const bounds = new google.maps.LatLngBounds();  
        console.log(places);
    
        places.forEach((place) => {
          if (!place.geometry || !place.geometry.location) {
            console.log("Returned place contains no geometry");
            return;
          }
  
          if (place.geometry.viewport) {
            // Only geocodes have viewport.
            bounds.union(place.geometry.viewport);
          } else {
            bounds.extend(place.geometry.location);
          }
        });
        map.fitBounds(bounds);
      });
    }
    
    window.initAutocomplete = initAutocomplete;